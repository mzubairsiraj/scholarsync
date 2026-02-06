using System;
using System.Collections.Generic;
using Npgsql;
using ScholarSync.Db_Connection;
using ScholarSync.Models;

namespace ScholarSync.Repositories
{
    /// <summary>
    /// Repository for Teacher database operations
    /// Follows Repository Pattern for separation of concerns
    /// </summary>
    public class TeacherRepository
    {
        /// <summary>
        /// Registers a new teacher in the system
        /// Creates user account and teacher record in a transaction
        /// </summary>
        /// <param name="teacher">Teacher model with basic information</param>
        /// <param name="email">Email for user account</param>
        /// <param name="password">Password for user account</param>
        /// <returns>Teacher ID if successful, null if failed</returns>
        public string RegisterTeacher(TeacherModel teacher, string email, string password)
        {
            NpgsqlConnection conn = null;
            NpgsqlTransaction transaction = null;

            try
            {
                conn = DbConnector.GetConnection();
                conn.Open();
                transaction = conn.BeginTransaction();

                // Step 1: Create User Account
                string userId = CreateUserAccount(conn, transaction, teacher.CNIC, teacher.FullName, email, password);
                if (string.IsNullOrEmpty(userId))
                {
                    transaction.Rollback();
                    return null;
                }

                teacher.UserId = userId;

                // Step 2: Create Teacher Record
                string teacherId = CreateTeacherRecord(conn, transaction, teacher);
                if (string.IsNullOrEmpty(teacherId))
                {
                    transaction.Rollback();
                    return null;
                }

                // Commit transaction
                transaction.Commit();
                return teacherId;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch { }
                }

                throw new Exception($"Error registering teacher: {ex.Message}", ex);
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates a user account for the teacher
        /// </summary>
        private string CreateUserAccount(NpgsqlConnection conn, NpgsqlTransaction transaction,
            string cnic, string name, string email, string password)
        {
            try
            {
                string query = @"INSERT INTO users (cnic, name, email, password, role) 
                               VALUES (@cnic, @name, @email, @password, 'Teacher'::user_role) 
                               RETURNING id";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@cnic", cnic);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@password", password);

                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating user account: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Creates the teacher record
        /// </summary>
        private string CreateTeacherRecord(NpgsqlConnection conn, NpgsqlTransaction transaction, TeacherModel teacher)
        {
            try
            {
                string query = @"INSERT INTO teachers (
                                   user_id, cnic, designation, specialization
                               ) VALUES (
                                   @user_id, @cnic, @designation, @specialization
                               ) RETURNING id";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@user_id", Guid.Parse(teacher.UserId));
                    cmd.Parameters.AddWithValue("@cnic", teacher.CNIC);
                    cmd.Parameters.AddWithValue("@designation", (object)teacher.Designation ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@specialization", (object)teacher.Specialization ?? DBNull.Value);

                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating teacher record: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a teacher by ID with complete information
        /// </summary>
        public TeacherViewModel GetTeacherById(string teacherId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT t.id, t.user_id, t.cnic, t.designation, 
                                   t.specialization, t.department_id, u.name, u.email
                                   FROM teachers t
                                   LEFT JOIN users u ON t.user_id = u.id
                                   WHERE t.id = @teacher_id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@teacher_id", Guid.Parse(teacherId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapToViewModel(reader);
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting teacher: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets all teachers with complete information
        /// </summary>
        public List<TeacherViewModel> GetAllTeachers()
        {
            List<TeacherViewModel> teachers = new List<TeacherViewModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT t.id, t.user_id, t.cnic, t.designation, 
                                   t.specialization, t.department_id, u.name, u.email
                                   FROM teachers t
                                   LEFT JOIN users u ON t.user_id = u.id
                                   ORDER BY u.name";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                teachers.Add(MapToViewModel(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting teachers: {ex.Message}", ex);
            }

            return teachers;
        }

        /// <summary>
        /// Searches for teachers by CNIC or Name
        /// </summary>
        public TeacherViewModel SearchTeacher(string searchTerm)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string searchCNIC = searchTerm.Replace("-", "");

                    string query = @"SELECT t.id, t.user_id, t.cnic, t.designation, 
                                   t.specialization, t.department_id, u.name, u.email
                                   FROM teachers t
                                   LEFT JOIN users u ON t.user_id = u.id
                                   WHERE t.cnic = @searchCNIC OR u.name ILIKE @searchName";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@searchCNIC", searchCNIC);
                        cmd.Parameters.AddWithValue("@searchName", $"%{searchTerm}%");

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return MapToViewModel(reader);
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching teacher: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates teacher information
        /// </summary>
        public bool UpdateTeacher(TeacherModel teacher, string email, string newPassword = null)
        {
            NpgsqlConnection conn = null;
            NpgsqlTransaction transaction = null;

            try
            {
                conn = DbConnector.GetConnection();
                conn.Open();
                transaction = conn.BeginTransaction();

                // Update User Account
                string userQuery = @"UPDATE users SET name = @name, email = @email";
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    userQuery += ", password = @password";
                }
                userQuery += " WHERE id = @user_id";

                using (NpgsqlCommand cmd = new NpgsqlCommand(userQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@name", teacher.FullName);
                    cmd.Parameters.AddWithValue("@email", email);
                    if (!string.IsNullOrWhiteSpace(newPassword))
                    {
                        cmd.Parameters.AddWithValue("@password", newPassword);
                    }
                    cmd.Parameters.AddWithValue("@user_id", Guid.Parse(teacher.UserId));
                    cmd.ExecuteNonQuery();
                }

                // Update Teacher Record
                string teacherQuery = @"UPDATE teachers SET 
                                       cnic = @cnic, 
                                       designation = @designation, 
                                       specialization = @specialization 
                                       WHERE id = @teacher_id";

                using (NpgsqlCommand cmd = new NpgsqlCommand(teacherQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@cnic", teacher.CNIC);
                    cmd.Parameters.AddWithValue("@designation", (object)teacher.Designation ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@specialization", (object)teacher.Specialization ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@teacher_id", Guid.Parse(teacher.Id));

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (transaction != null)
                {
                    try { transaction.Rollback(); } catch { }
                }

                throw new Exception($"Error updating teacher: {ex.Message}", ex);
            }
            finally
            {
                if (transaction != null) transaction.Dispose();
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// Deletes a teacher and their associated user account
        /// </summary>
        public bool DeleteTeacher(string teacherId, string userId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    // Delete teacher - cascade will handle related records
                    string deleteTeacherQuery = "DELETE FROM teachers WHERE id = @teacher_id";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteTeacherQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@teacher_id", Guid.Parse(teacherId));
                        cmd.ExecuteNonQuery();
                    }

                    // Delete user account
                    string deleteUserQuery = "DELETE FROM users WHERE id = @user_id";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteUserQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@user_id", Guid.Parse(userId));
                        cmd.ExecuteNonQuery();
                    }

                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting teacher: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Helper method to map database reader to TeacherViewModel
        /// </summary>
        private TeacherViewModel MapToViewModel(NpgsqlDataReader reader)
        {
            return new TeacherViewModel
            {
                Id = reader["id"].ToString(),
                UserId = reader["user_id"].ToString(),
                CNIC = reader["cnic"].ToString(),
                FullName = reader["name"] != DBNull.Value ? reader["name"].ToString() : null,
                Designation = reader["designation"] != DBNull.Value ? reader["designation"].ToString() : null,
                Specialization = reader["specialization"] != DBNull.Value ? reader["specialization"].ToString() : null,
                DepartmentId = reader["department_id"] != DBNull.Value ? reader["department_id"].ToString() : null,
                Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : null
            };
        }
    }
}
