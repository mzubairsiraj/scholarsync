using System;
using System.Collections.Generic;
using Npgsql;
using ScholarSync.Db_Connection;
using ScholarSync.Models;

namespace ScholarSync.Repositories
{
    
    public class EnrollmentRepository
    {
        
        public string AddEnrollment(EnrollmentModel enrollment)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO enrollments (student_id, subject_id, semester_id)
                                   VALUES (@student_id, @subject_id, @semester_id)
                                   RETURNING id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", Guid.Parse(enrollment.StudentId));
                        cmd.Parameters.AddWithValue("@subject_id", Guid.Parse(enrollment.SubjectId));
                        
                        if (!string.IsNullOrEmpty(enrollment.SemesterId))
                            cmd.Parameters.AddWithValue("@semester_id", Guid.Parse(enrollment.SemesterId));
                        else
                            cmd.Parameters.AddWithValue("@semester_id", DBNull.Value);

                        object result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding enrollment: {ex.Message}", ex);
            }
        }

        
        public string AddOrUpdateResult(ResultModel result)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    // Check if result exists
                    string checkQuery = "SELECT id FROM results WHERE enrollment_id = @enrollment_id";
                    using (NpgsqlCommand checkCmd = new NpgsqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@enrollment_id", Guid.Parse(result.EnrollmentId));
                        object existingId = checkCmd.ExecuteScalar();

                        if (existingId != null)
                        {
                            // Update existing result
                            string updateQuery = @"UPDATE results 
                                                 SET exam_type = @exam_type,
                                                     total_marks = @total_marks,
                                                     obtained_marks = @obtained_marks,
                                                     gpa = @gpa,
                                                     grade_letter = @grade_letter,
                                                     remarks = @remarks
                                                 WHERE enrollment_id = @enrollment_id
                                                 RETURNING id";

                            using (NpgsqlCommand cmd = new NpgsqlCommand(updateQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@enrollment_id", Guid.Parse(result.EnrollmentId));
                                cmd.Parameters.AddWithValue("@exam_type", !string.IsNullOrEmpty(result.ExamType) ? result.ExamType : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@total_marks", result.TotalMarks);
                                cmd.Parameters.AddWithValue("@obtained_marks", result.ObtainedMarks);
                                cmd.Parameters.AddWithValue("@gpa", result.GPA.HasValue ? (object)result.GPA.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@grade_letter", !string.IsNullOrEmpty(result.GradeLetter) ? result.GradeLetter : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@remarks", !string.IsNullOrEmpty(result.Remarks) ? result.Remarks : (object)DBNull.Value);

                                return cmd.ExecuteScalar()?.ToString();
                            }
                        }
                        else
                        {
                            // Insert new result
                            string insertQuery = @"INSERT INTO results (enrollment_id, exam_type, total_marks, obtained_marks, gpa, grade_letter, remarks)
                                                 VALUES (@enrollment_id, @exam_type, @total_marks, @obtained_marks, @gpa, @grade_letter, @remarks)
                                                 RETURNING id";

                            using (NpgsqlCommand cmd = new NpgsqlCommand(insertQuery, conn))
                            {
                                cmd.Parameters.AddWithValue("@enrollment_id", Guid.Parse(result.EnrollmentId));
                                cmd.Parameters.AddWithValue("@exam_type", !string.IsNullOrEmpty(result.ExamType) ? result.ExamType : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@total_marks", result.TotalMarks);
                                cmd.Parameters.AddWithValue("@obtained_marks", result.ObtainedMarks);
                                cmd.Parameters.AddWithValue("@gpa", result.GPA.HasValue ? (object)result.GPA.Value : DBNull.Value);
                                cmd.Parameters.AddWithValue("@grade_letter", !string.IsNullOrEmpty(result.GradeLetter) ? result.GradeLetter : (object)DBNull.Value);
                                cmd.Parameters.AddWithValue("@remarks", !string.IsNullOrEmpty(result.Remarks) ? result.Remarks : (object)DBNull.Value);

                                return cmd.ExecuteScalar()?.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding/updating result: {ex.Message}", ex);
            }
        }

       
        public List<EnrollmentViewModel> GetAllEnrollments()
        {
            List<EnrollmentViewModel> enrollments = new List<EnrollmentViewModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT 
                                       e.id, e.student_id, e.subject_id, e.semester_id,
                                       s.roll_number, u.name as student_name, st.cnic,
                                       sub.name as subject_name, sub.code as subject_code,
                                       d.name as department_name,
                                       sem.name as semester_name,
                                       r.id as result_id, r.exam_type, r.total_marks, r.obtained_marks,
                                       r.gpa, r.grade_letter, r.remarks
                                   FROM enrollments e
                                   INNER JOIN students s ON e.student_id = s.id
                                   INNER JOIN students st ON s.id = st.id
                                   INNER JOIN users u ON st.user_id = u.id
                                   INNER JOIN subjects sub ON e.subject_id = sub.id
                                   INNER JOIN departments d ON sub.dept_id = d.id
                                   LEFT JOIN semesters sem ON e.semester_id = sem.id
                                   INNER JOIN results r ON e.id = r.enrollment_id
                                   ORDER BY s.roll_number, sub.code";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                enrollments.Add(MapToViewModel(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting enrollments: {ex.Message}", ex);
            }

            return enrollments;
        }

        public List<EnrollmentViewModel> GetEnrollmentsByStudent(string rollNumber)
        {
            List<EnrollmentViewModel> enrollments = new List<EnrollmentViewModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT 
                                       e.id, e.student_id, e.subject_id, e.semester_id,
                                       s.roll_number, u.name as student_name, st.cnic,
                                       sub.name as subject_name, sub.code as subject_code,
                                       d.name as department_name,
                                       sem.name as semester_name,
                                       r.id as result_id, r.exam_type, r.total_marks, r.obtained_marks,
                                       r.gpa, r.grade_letter, r.remarks
                                   FROM enrollments e
                                   INNER JOIN students s ON e.student_id = s.id
                                   INNER JOIN students st ON s.id = st.id
                                   INNER JOIN users u ON st.user_id = u.id
                                   INNER JOIN subjects sub ON e.subject_id = sub.id
                                   INNER JOIN departments d ON sub.dept_id = d.id
                                   LEFT JOIN semesters sem ON e.semester_id = sem.id
                                   INNER JOIN results r ON e.id = r.enrollment_id
                                   WHERE s.roll_number = @roll_number
                                   ORDER BY sub.code";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@roll_number", rollNumber);

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                enrollments.Add(MapToViewModel(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting student enrollments: {ex.Message}", ex);
            }

            return enrollments;
        }

        
        public EnrollmentViewModel GetEnrollmentById(string enrollmentId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT 
                                       e.id, e.student_id, e.subject_id, e.semester_id,
                                       s.roll_number, u.name as student_name, st.cnic,
                                       sub.name as subject_name, sub.code as subject_code,
                                       d.name as department_name,
                                       sem.name as semester_name,
                                       r.id as result_id, r.exam_type, r.total_marks, r.obtained_marks,
                                       r.gpa, r.grade_letter, r.remarks
                                   FROM enrollments e
                                   INNER JOIN students s ON e.student_id = s.id
                                   INNER JOIN students st ON s.id = st.id
                                   INNER JOIN users u ON st.user_id = u.id
                                   INNER JOIN subjects sub ON e.subject_id = sub.id
                                   INNER JOIN departments d ON sub.dept_id = d.id
                                   LEFT JOIN semesters sem ON e.semester_id = sem.id
                                   INNER JOIN results r ON e.id = r.enrollment_id
                                   WHERE e.id = @enrollment_id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@enrollment_id", Guid.Parse(enrollmentId));

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
                throw new Exception($"Error getting enrollment: {ex.Message}", ex);
            }
        }

        
        public EnrollmentViewModel GetEnrollmentByStudentSubjectSemester(string studentId, string subjectId, string semesterId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT 
                                       e.id, e.student_id, e.subject_id, e.semester_id,
                                       s.roll_number, u.name as student_name, st.cnic,
                                       sub.name as subject_name, sub.code as subject_code,
                                       d.name as department_name,
                                       sem.name as semester_name,
                                       r.id as result_id, r.exam_type, r.total_marks, r.obtained_marks,
                                       r.gpa, r.grade_letter, r.remarks
                                   FROM enrollments e
                                   INNER JOIN students s ON e.student_id = s.id
                                   INNER JOIN students st ON s.id = st.id
                                   INNER JOIN users u ON st.user_id = u.id
                                   INNER JOIN subjects sub ON e.subject_id = sub.id
                                   INNER JOIN departments d ON sub.dept_id = d.id
                                   LEFT JOIN semesters sem ON e.semester_id = sem.id
                                   LEFT JOIN results r ON e.id = r.enrollment_id
                                   WHERE e.student_id = @student_id 
                                     AND e.subject_id = @subject_id 
                                     AND e.semester_id = @semester_id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", Guid.Parse(studentId));
                        cmd.Parameters.AddWithValue("@subject_id", Guid.Parse(subjectId));
                        cmd.Parameters.AddWithValue("@semester_id", Guid.Parse(semesterId));

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
                throw new Exception($"Error getting enrollment: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets current semester
        /// </summary>
        public string GetCurrentSemesterId()
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT id FROM semesters WHERE is_current = true LIMIT 1";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        object result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting current semester: {ex.Message}", ex);
            }
        }

        public bool DeleteResult(string resultId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = "DELETE FROM results WHERE id = @result_id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@result_id", Guid.Parse(resultId));
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting result: {ex.Message}", ex);
            }
        }

        
        public EnrollmentViewModel GetResultById(string resultId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT 
                                       e.id, e.student_id, e.subject_id, e.semester_id,
                                       s.roll_number, u.name as student_name, st.cnic,
                                       sub.name as subject_name, sub.code as subject_code,
                                       d.name as department_name,
                                       sem.name as semester_name,
                                       r.id as result_id, r.exam_type, r.total_marks, r.obtained_marks,
                                       r.gpa, r.grade_letter, r.remarks
                                   FROM results r
                                   INNER JOIN enrollments e ON r.enrollment_id = e.id
                                   INNER JOIN students s ON e.student_id = s.id
                                   INNER JOIN students st ON s.id = st.id
                                   INNER JOIN users u ON st.user_id = u.id
                                   INNER JOIN subjects sub ON e.subject_id = sub.id
                                   INNER JOIN departments d ON sub.dept_id = d.id
                                   LEFT JOIN semesters sem ON e.semester_id = sem.id
                                   WHERE r.id = @result_id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@result_id", Guid.Parse(resultId));

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
                throw new Exception($"Error getting result: {ex.Message}", ex);
            }
        }

        
        public bool UpdateResultById(string resultId, ResultModel result)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE results 
                                   SET exam_type = @exam_type,
                                       total_marks = @total_marks,
                                       obtained_marks = @obtained_marks,
                                       gpa = @gpa,
                                       grade_letter = @grade_letter,
                                       remarks = @remarks
                                   WHERE id = @result_id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@result_id", Guid.Parse(resultId));
                        cmd.Parameters.AddWithValue("@exam_type", !string.IsNullOrEmpty(result.ExamType) ? result.ExamType : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@total_marks", result.TotalMarks);
                        cmd.Parameters.AddWithValue("@obtained_marks", result.ObtainedMarks);
                        cmd.Parameters.AddWithValue("@gpa", result.GPA.HasValue ? (object)result.GPA.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@grade_letter", !string.IsNullOrEmpty(result.GradeLetter) ? result.GradeLetter : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@remarks", !string.IsNullOrEmpty(result.Remarks) ? result.Remarks : (object)DBNull.Value);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating result: {ex.Message}", ex);
            }
        }

        
        private EnrollmentViewModel MapToViewModel(NpgsqlDataReader reader)
        {
            return new EnrollmentViewModel
            {
                Id = reader["id"].ToString(),
                StudentId = reader["student_id"].ToString(),
                SubjectId = reader["subject_id"].ToString(),
                SemesterId = reader["semester_id"] != DBNull.Value ? reader["semester_id"].ToString() : null,
                StudentRollNumber = reader["roll_number"].ToString(),
                StudentName = reader["student_name"].ToString(),
                StudentCNIC = reader["cnic"].ToString(),
                SubjectName = reader["subject_name"].ToString(),
                SubjectCode = reader["subject_code"].ToString(),
                DepartmentName = reader["department_name"].ToString(),
                SemesterName = reader["semester_name"] != DBNull.Value ? reader["semester_name"].ToString() : null,
                ResultId = reader["result_id"] != DBNull.Value ? reader["result_id"].ToString() : null,
                ExamType = reader["exam_type"] != DBNull.Value ? reader["exam_type"].ToString() : null,
                TotalMarks = reader["total_marks"] != DBNull.Value ? Convert.ToInt32(reader["total_marks"]) : (int?)null,
                ObtainedMarks = reader["obtained_marks"] != DBNull.Value ? Convert.ToDecimal(reader["obtained_marks"]) : (decimal?)null,
                GPA = reader["gpa"] != DBNull.Value ? Convert.ToDecimal(reader["gpa"]) : (decimal?)null,
                GradeLetter = reader["grade_letter"] != DBNull.Value ? reader["grade_letter"].ToString() : null,
                Remarks = reader["remarks"] != DBNull.Value ? reader["remarks"].ToString() : null
            };
        }
    }
}

