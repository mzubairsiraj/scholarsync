using System;
using System.Collections.Generic;
using Npgsql;
using ScholarSync.Db_Connection;
using ScholarSync.Models;

namespace ScholarSync.Repositories
{
    
    public class StudentRepository
    {
        
        public string RegisterStudent(StudentModel student, string email, string password, GuardianModel guardian = null)
        {
            NpgsqlConnection conn = null;
            NpgsqlTransaction transaction = null;

            try
            {
                conn = DbConnector.GetConnection();
                conn.Open();
                transaction = conn.BeginTransaction();

               
                string userId = CreateUserAccount(conn, transaction, student.CNIC, student.FullName, email, password);
                if (string.IsNullOrEmpty(userId))
                {
                    transaction.Rollback();
                    return null;
                }

                student.UserId = userId;

                
                string studentId = CreateStudentRecord(conn, transaction, student);
                if (string.IsNullOrEmpty(studentId))
                {
                    transaction.Rollback();
                    return null;
                }

               
                if (guardian != null)
                {
                    guardian.StudentId = studentId;
                    if (!CreateGuardianRecord(conn, transaction, guardian))
                    {
                        transaction.Rollback();
                        return null;
                    }
                }

                // Commit transaction
                transaction.Commit();
                return studentId;
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

                throw new Exception($"Error registering student: {ex.Message}", ex);
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

        
        private string CreateUserAccount(NpgsqlConnection conn, NpgsqlTransaction transaction, 
            string cnic, string name, string email, string password)
        {
            try
            {
                string query = @"INSERT INTO users (cnic, name, email, password, role) 
                               VALUES (@cnic, @name, @email, @password, 'Student'::user_role) 
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

       
        private string CreateStudentRecord(NpgsqlConnection conn, NpgsqlTransaction transaction, StudentModel student)
        {
            try
            {
                string query = @"INSERT INTO students (
                                   user_id, program_id, cnic, roll_number, full_name, 
                                   date_of_birth, gender, blood_group, religion, 
                                   nationality, domicile_district, mobile_no, whatsapp_no
                               ) VALUES (
                                   @user_id, @program_id, @cnic, @roll_number, @full_name,
                                   @date_of_birth, @gender::gender_type, @blood_group, @religion,
                                   @nationality, @domicile_district, @mobile_no, @whatsapp_no
                               ) RETURNING id";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@user_id", Guid.Parse(student.UserId));
                    cmd.Parameters.AddWithValue("@program_id", Guid.Parse(student.ProgramId));
                    cmd.Parameters.AddWithValue("@cnic", student.CNIC);
                    cmd.Parameters.AddWithValue("@roll_number", student.RollNumber);
                    cmd.Parameters.AddWithValue("@full_name", student.FullName);
                    cmd.Parameters.AddWithValue("@date_of_birth", student.DateOfBirth);
                    cmd.Parameters.AddWithValue("@gender", student.Gender);
                    cmd.Parameters.AddWithValue("@blood_group", (object)student.BloodGroup ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@religion", (object)student.Religion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@nationality", (object)student.Nationality ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@domicile_district", (object)student.DomicileDistrict ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@mobile_no", student.MobileNo);
                    cmd.Parameters.AddWithValue("@whatsapp_no", (object)student.WhatsappNo ?? DBNull.Value);

                    object result = cmd.ExecuteScalar();
                    return result?.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating student record: {ex.Message}", ex);
            }
        }

        private bool CreateGuardianRecord(NpgsqlConnection conn, NpgsqlTransaction transaction, GuardianModel guardian)
        {
            try
            {
                string query = @"INSERT INTO guardians (
                                   student_id, relation, full_name, cnic, 
                                   mobile_no, address, is_deceased
                               ) VALUES (
                                   @student_id, @relation::guardian_relation, @full_name, @cnic,
                                   @mobile_no, @address, @is_deceased
                               )";

                using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@student_id", Guid.Parse(guardian.StudentId));
                    cmd.Parameters.AddWithValue("@relation", guardian.Relation);
                    cmd.Parameters.AddWithValue("@full_name", guardian.FullName);
                    cmd.Parameters.AddWithValue("@cnic", (object)guardian.CNIC ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@mobile_no", guardian.MobileNo);
                    cmd.Parameters.AddWithValue("@address", (object)guardian.Address ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@is_deceased", guardian.IsDeceased);

                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating guardian record: {ex.Message}", ex);
            }
        }

        
        public StudentViewModel GetStudentById(string studentId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT s.id, s.user_id, s.cnic, s.roll_number, s.full_name, 
                                   s.date_of_birth, s.gender::text, s.blood_group, s.religion, 
                                   s.nationality, s.domicile_district, s.mobile_no, s.whatsapp_no, 
                                   s.program_id, p.name as program_name, u.email
                                   FROM students s
                                   LEFT JOIN programs p ON s.program_id = p.id
                                   LEFT JOIN users u ON s.user_id = u.id
                                   WHERE s.id = @student_id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", Guid.Parse(studentId));

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
                throw new Exception($"Error getting student: {ex.Message}", ex);
            }
        }

        
        public List<StudentViewModel> GetAllStudents()
        {
            List<StudentViewModel> students = new List<StudentViewModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT s.id, s.user_id, s.cnic, s.roll_number, s.full_name, 
                                   s.date_of_birth, s.gender::text, s.blood_group, s.religion, 
                                   s.nationality, s.domicile_district, s.mobile_no, s.whatsapp_no, 
                                   s.program_id, p.name as program_name, u.email
                                   FROM students s
                                   LEFT JOIN programs p ON s.program_id = p.id
                                   LEFT JOIN users u ON s.user_id = u.id
                                   ORDER BY s.roll_number";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                students.Add(MapToViewModel(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting students: {ex.Message}", ex);
            }

            return students;
        }

     
        public StudentViewModel SearchStudent(string searchTerm)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string searchCNIC = searchTerm.Replace("-", "");

                    string query = @"SELECT s.id, s.user_id, s.cnic, s.roll_number, s.full_name, 
                                   s.date_of_birth, s.gender::text, s.blood_group, s.religion, 
                                   s.nationality, s.domicile_district, s.mobile_no, s.whatsapp_no, 
                                   s.program_id, p.name as program_name, u.email
                                   FROM students s
                                   LEFT JOIN programs p ON s.program_id = p.id
                                   LEFT JOIN users u ON s.user_id = u.id
                                   WHERE s.cnic = @searchCNIC OR s.roll_number = @searchRoll";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@searchCNIC", searchCNIC);
                        cmd.Parameters.AddWithValue("@searchRoll", searchTerm);

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
                throw new Exception($"Error searching student: {ex.Message}", ex);
            }
        }

        
        public List<StudentViewModel> SearchStudents(string searchTerm)
        {
            List<StudentViewModel> students = new List<StudentViewModel>();
            
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string searchCNIC = searchTerm.Replace("-", "");

                    string query = @"SELECT s.id, s.user_id, s.cnic, s.roll_number, s.full_name, 
                                   s.date_of_birth, s.gender::text, s.blood_group, s.religion, 
                                   s.nationality, s.domicile_district, s.mobile_no, s.whatsapp_no, 
                                   s.program_id, p.name as program_name, u.email
                                   FROM students s
                                   LEFT JOIN programs p ON s.program_id = p.id
                                   LEFT JOIN users u ON s.user_id = u.id
                                   WHERE s.cnic = @searchCNIC OR s.roll_number = @searchRoll OR s.full_name ILIKE @searchName";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@searchCNIC", searchCNIC);
                        cmd.Parameters.AddWithValue("@searchRoll", searchTerm);
                        cmd.Parameters.AddWithValue("@searchName", "%" + searchTerm + "%");

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                students.Add(MapToViewModel(reader));
                            }
                        }
                    }
                }

                return students;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error searching students: {ex.Message}", ex);
            }
        }

        public bool UpdateStudent(StudentModel student, string email, string newPassword = null)
        {
            NpgsqlConnection conn = null;
            NpgsqlTransaction transaction = null;

            try
            {
                conn = DbConnector.GetConnection();
                conn.Open();
                transaction = conn.BeginTransaction();

                
                string userQuery = @"UPDATE users SET name = @name, email = @email";
                if (!string.IsNullOrWhiteSpace(newPassword))
                {
                    userQuery += ", password = @password";
                }
                userQuery += " WHERE id = @user_id";

                using (NpgsqlCommand cmd = new NpgsqlCommand(userQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@name", student.FullName);
                    cmd.Parameters.AddWithValue("@email", email);
                    if (!string.IsNullOrWhiteSpace(newPassword))
                    {
                        cmd.Parameters.AddWithValue("@password", newPassword);
                    }
                    cmd.Parameters.AddWithValue("@user_id", Guid.Parse(student.UserId));
                    cmd.ExecuteNonQuery();
                }

                string studentQuery = @"UPDATE students SET 
                                       program_id = @program_id, 
                                       cnic = @cnic, 
                                       roll_number = @roll_number, 
                                       full_name = @full_name, 
                                       date_of_birth = @date_of_birth, 
                                       gender = @gender::gender_type, 
                                       blood_group = @blood_group, 
                                       religion = @religion, 
                                       nationality = @nationality, 
                                       domicile_district = @domicile_district, 
                                       mobile_no = @mobile_no, 
                                       whatsapp_no = @whatsapp_no 
                                       WHERE id = @student_id";

                using (NpgsqlCommand cmd = new NpgsqlCommand(studentQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@program_id", Guid.Parse(student.ProgramId));
                    cmd.Parameters.AddWithValue("@cnic", student.CNIC);
                    cmd.Parameters.AddWithValue("@roll_number", student.RollNumber);
                    cmd.Parameters.AddWithValue("@full_name", student.FullName);
                    cmd.Parameters.AddWithValue("@date_of_birth", student.DateOfBirth);
                    cmd.Parameters.AddWithValue("@gender", student.Gender);
                    cmd.Parameters.AddWithValue("@blood_group", (object)student.BloodGroup ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@religion", (object)student.Religion ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@nationality", (object)student.Nationality ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@domicile_district", (object)student.DomicileDistrict ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@mobile_no", student.MobileNo);
                    cmd.Parameters.AddWithValue("@whatsapp_no", (object)student.WhatsappNo ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@student_id", Guid.Parse(student.Id));

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

                throw new Exception($"Error updating student: {ex.Message}", ex);
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

       
        public bool DeleteStudent(string studentId, string userId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    // Delete student - cascade will handle guardians, enrollments, etc.
                    string deleteStudentQuery = "DELETE FROM students WHERE id = @student_id";
                    using (NpgsqlCommand cmd = new NpgsqlCommand(deleteStudentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", Guid.Parse(studentId));
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
                throw new Exception($"Error deleting student: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Helper method to map database reader to StudentViewModel
        /// </summary>
        private StudentViewModel MapToViewModel(NpgsqlDataReader reader)
        {
            return new StudentViewModel
            {
                Id = reader["id"].ToString(),
                UserId = reader["user_id"].ToString(),
                CNIC = reader["cnic"].ToString(),
                RollNumber = reader["roll_number"].ToString(),
                FullName = reader["full_name"].ToString(),
                DateOfBirth = Convert.ToDateTime(reader["date_of_birth"]),
                Gender = reader["gender"].ToString(),
                BloodGroup = reader["blood_group"] != DBNull.Value ? reader["blood_group"].ToString() : null,
                Religion = reader["religion"] != DBNull.Value ? reader["religion"].ToString() : null,
                Nationality = reader["nationality"] != DBNull.Value ? reader["nationality"].ToString() : null,
                DomicileDistrict = reader["domicile_district"] != DBNull.Value ? reader["domicile_district"].ToString() : null,
                MobileNo = reader["mobile_no"].ToString(),
                WhatsappNo = reader["whatsapp_no"] != DBNull.Value ? reader["whatsapp_no"].ToString() : null,
                ProgramId = reader["program_id"].ToString(),
                ProgramName = reader["program_name"] != DBNull.Value ? reader["program_name"].ToString() : null,
                Email = reader["email"] != DBNull.Value ? reader["email"].ToString() : null
            };
        }
    }
}
