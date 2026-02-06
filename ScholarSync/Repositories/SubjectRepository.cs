using System;
using System.Collections.Generic;
using Npgsql;
using ScholarSync.Db_Connection;
using ScholarSync.Models;

namespace ScholarSync.Repositories
{
    /// <summary>
    /// Repository for managing subjects
    /// Handles all database operations for subjects table
    /// </summary>
    public class SubjectRepository
    {
        /// <summary>
        /// Gets all subjects for a specific department
        /// </summary>
        public List<SubjectModel> GetSubjectsByDepartment(string departmentId)
        {
            List<SubjectModel> subjects = new List<SubjectModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT id, dept_id, name, code, credit_hours
                                   FROM subjects
                                   WHERE dept_id = @dept_id
                                   ORDER BY code";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dept_id", Guid.Parse(departmentId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                subjects.Add(new SubjectModel
                                {
                                    Id = reader["id"].ToString(),
                                    DeptId = reader["dept_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Code = reader["code"].ToString(),
                                    CreditHours = reader["credit_hours"] != DBNull.Value ? Convert.ToInt32(reader["credit_hours"]) : 3
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting subjects by department: {ex.Message}", ex);
            }

            return subjects;
        }

        /// <summary>
        /// Gets all subjects
        /// </summary>
        public List<SubjectModel> GetAllSubjects()
        {
            List<SubjectModel> subjects = new List<SubjectModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT id, dept_id, name, code, credit_hours
                                   FROM subjects
                                   ORDER BY code";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                subjects.Add(new SubjectModel
                                {
                                    Id = reader["id"].ToString(),
                                    DeptId = reader["dept_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Code = reader["code"].ToString(),
                                    CreditHours = reader["credit_hours"] != DBNull.Value ? Convert.ToInt32(reader["credit_hours"]) : 3
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting all subjects: {ex.Message}", ex);
            }

            return subjects;
        }

        /// <summary>
        /// Gets subjects for a student's program
        /// </summary>
        public List<SubjectModel> GetSubjectsForStudentProgram(string studentId)
        {
            List<SubjectModel> subjects = new List<SubjectModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT s.id, s.dept_id, s.name, s.code, s.credit_hours
                                   FROM subjects s
                                   INNER JOIN programs p ON s.dept_id = p.dept_id
                                   INNER JOIN students st ON st.program_id = p.id
                                   WHERE st.id = @student_id
                                   ORDER BY s.code";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", Guid.Parse(studentId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                subjects.Add(new SubjectModel
                                {
                                    Id = reader["id"].ToString(),
                                    DeptId = reader["dept_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Code = reader["code"].ToString(),
                                    CreditHours = reader["credit_hours"] != DBNull.Value ? Convert.ToInt32(reader["credit_hours"]) : 3
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting subjects for student program: {ex.Message}", ex);
            }

            return subjects;
        }

        /// <summary>
        /// Adds a new subject
        /// </summary>
        public string AddSubject(SubjectModel subject)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO subjects (dept_id, name, code, credit_hours)
                                   VALUES (@dept_id, @name, @code, @credit_hours)
                                   RETURNING id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dept_id", Guid.Parse(subject.DeptId));
                        cmd.Parameters.AddWithValue("@name", subject.Name);
                        cmd.Parameters.AddWithValue("@code", subject.Code);
                        cmd.Parameters.AddWithValue("@credit_hours", subject.CreditHours > 0 ? subject.CreditHours : 3);

                        object result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding subject: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates an existing subject
        /// </summary>
        public bool UpdateSubject(SubjectModel subject)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE subjects
                                   SET dept_id = @dept_id,
                                       name = @name,
                                       code = @code,
                                       credit_hours = @credit_hours
                                   WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.Parse(subject.Id));
                        cmd.Parameters.AddWithValue("@dept_id", Guid.Parse(subject.DeptId));
                        cmd.Parameters.AddWithValue("@name", subject.Name);
                        cmd.Parameters.AddWithValue("@code", subject.Code);
                        cmd.Parameters.AddWithValue("@credit_hours", subject.CreditHours > 0 ? subject.CreditHours : 3);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating subject: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes a subject
        /// </summary>
        public bool DeleteSubject(string subjectId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = "DELETE FROM subjects WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.Parse(subjectId));
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting subject: {ex.Message}", ex);
            }
        }
    }
}
