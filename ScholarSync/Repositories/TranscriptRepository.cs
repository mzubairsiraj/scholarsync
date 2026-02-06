using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using ScholarSync.Db_Connection;
using ScholarSync.Models;

namespace ScholarSync.Repositories
{
    /// <summary>
    /// Repository for fetching transcript data
    /// </summary>
    public class TranscriptRepository
    {
        /// <summary>
        /// Gets complete transcript data for a student (all semesters)
        /// </summary>
        public TranscriptModel GetFullTranscript(string studentId)
        {
            try
            {
                TranscriptModel transcript = new TranscriptModel();

                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    // Get student information
                    string studentQuery = @"SELECT s.id, u.name, s.roll_number, s.cnic, p.name as program_name, d.name as department_name
                                          FROM students s
                                          INNER JOIN users u ON s.user_id = u.id
                                          INNER JOIN programs p ON s.program_id = p.id
                                          INNER JOIN departments d ON p.dept_id = d.id
                                          WHERE s.id = @student_id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(studentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", Guid.Parse(studentId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                transcript.StudentId = reader["id"].ToString();
                                transcript.StudentName = reader["name"].ToString();
                                transcript.RollNumber = reader["roll_number"].ToString();
                                transcript.CNIC = reader["cnic"].ToString();
                                transcript.Program = reader["program_name"].ToString();
                                transcript.Department = reader["department_name"].ToString();
                            }
                            else
                            {
                                return null; // Student not found
                            }
                        }
                    }

                    // Get all semester data with results
                    string semesterQuery = @"SELECT DISTINCT sem.id, sem.name, sem.start_date, sem.end_date
                                           FROM enrollments e
                                           INNER JOIN semesters sem ON e.semester_id = sem.id
                                           INNER JOIN results r ON e.id = r.enrollment_id
                                           WHERE e.student_id = @student_id
                                           ORDER BY sem.start_date";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(semesterQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", Guid.Parse(studentId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SemesterTranscriptData semData = new SemesterTranscriptData
                                {
                                    SemesterId = reader["id"].ToString(),
                                    SemesterName = reader["name"].ToString(),
                                    StartDate = reader["start_date"] != DBNull.Value ? (DateTime?)reader["start_date"] : null,
                                    EndDate = reader["end_date"] != DBNull.Value ? (DateTime?)reader["end_date"] : null
                                };
                                transcript.Semesters.Add(semData);
                            }
                        }
                    }

                    // Get subject results for each semester
                    foreach (var semester in transcript.Semesters)
                    {
                        string resultsQuery = @"SELECT sub.code as subject_code, sub.name as subject_name, 
                                              sub.credit_hours, r.total_marks, r.obtained_marks, r.gpa, r.grade_letter
                                              FROM enrollments e
                                              INNER JOIN subjects sub ON e.subject_id = sub.id
                                              INNER JOIN results r ON e.id = r.enrollment_id
                                              WHERE e.student_id = @student_id AND e.semester_id = @semester_id
                                              ORDER BY sub.code";

                        using (NpgsqlCommand cmd = new NpgsqlCommand(resultsQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@student_id", Guid.Parse(studentId));
                            cmd.Parameters.AddWithValue("@semester_id", Guid.Parse(semester.SemesterId));

                            using (NpgsqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    int totalMarks = Convert.ToInt32(reader["total_marks"]);
                                    decimal obtainedMarks = Convert.ToDecimal(reader["obtained_marks"]);
                                    decimal percentage = (obtainedMarks / totalMarks) * 100;

                                    SubjectResultData subjectData = new SubjectResultData
                                    {
                                        SubjectCode = reader["subject_code"].ToString(),
                                        SubjectName = reader["subject_name"].ToString(),
                                        CreditHours = reader["credit_hours"] != DBNull.Value ? Convert.ToInt32(reader["credit_hours"]) : 3,
                                        TotalMarks = totalMarks,
                                        ObtainedMarks = obtainedMarks,
                                        Percentage = percentage,
                                        GPA = reader["gpa"] != DBNull.Value ? Convert.ToDecimal(reader["gpa"]) : 0,
                                        GradeLetter = reader["grade_letter"]?.ToString() ?? "F"
                                    };
                                    semester.Subjects.Add(subjectData);
                                }
                            }
                        }

                        // Calculate semester GPA and credit hours
                        if (semester.Subjects.Count > 0)
                        {
                            semester.SemesterCreditHours = semester.Subjects.Sum(s => s.CreditHours);
                            decimal totalGradePoints = semester.Subjects.Sum(s => s.GPA * s.CreditHours);
                            semester.SemesterGPA = semester.SemesterCreditHours > 0 ? 
                                totalGradePoints / semester.SemesterCreditHours : 0;
                        }
                    }

                    // Calculate overall CGPA
                    if (transcript.Semesters.Count > 0)
                    {
                        transcript.TotalCreditHours = transcript.Semesters.Sum(s => s.SemesterCreditHours);
                        decimal totalGradePoints = transcript.Semesters.Sum(s => s.SemesterGPA * s.SemesterCreditHours);
                        transcript.OverallCGPA = transcript.TotalCreditHours > 0 ? 
                            totalGradePoints / transcript.TotalCreditHours : 0;
                    }
                }

                return transcript;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching full transcript: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets transcript data for a specific semester
        /// </summary>
        public TranscriptModel GetSemesterTranscript(string studentId, string semesterId)
        {
            try
            {
                TranscriptModel transcript = new TranscriptModel();

                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    // Get student information
                    string studentQuery = @"SELECT s.id, u.name, s.roll_number, s.cnic, p.name as program_name, d.name as department_name
                                          FROM students s
                                          INNER JOIN users u ON s.user_id = u.id
                                          INNER JOIN programs p ON s.program_id = p.id
                                          INNER JOIN departments d ON p.dept_id = d.id
                                          WHERE s.id = @student_id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(studentQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", Guid.Parse(studentId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                transcript.StudentId = reader["id"].ToString();
                                transcript.StudentName = reader["name"].ToString();
                                transcript.RollNumber = reader["roll_number"].ToString();
                                transcript.CNIC = reader["cnic"].ToString();
                                transcript.Program = reader["program_name"].ToString();
                                transcript.Department = reader["department_name"].ToString();
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }

                    // Get semester data
                    string semesterQuery = @"SELECT id, name, start_date, end_date
                                           FROM semesters
                                           WHERE id = @semester_id";

                    SemesterTranscriptData semData = new SemesterTranscriptData();

                    using (NpgsqlCommand cmd = new NpgsqlCommand(semesterQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@semester_id", Guid.Parse(semesterId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                semData.SemesterId = reader["id"].ToString();
                                semData.SemesterName = reader["name"].ToString();
                                semData.StartDate = reader["start_date"] != DBNull.Value ? (DateTime?)reader["start_date"] : null;
                                semData.EndDate = reader["end_date"] != DBNull.Value ? (DateTime?)reader["end_date"] : null;
                            }
                            else
                            {
                                return null;
                            }
                        }
                    }

                    // Get subject results for the semester
                    string resultsQuery = @"SELECT sub.code as subject_code, sub.name as subject_name, 
                                          sub.credit_hours, r.total_marks, r.obtained_marks, r.gpa, r.grade_letter
                                          FROM enrollments e
                                          INNER JOIN subjects sub ON e.subject_id = sub.id
                                          INNER JOIN results r ON e.id = r.enrollment_id
                                          WHERE e.student_id = @student_id AND e.semester_id = @semester_id
                                          ORDER BY sub.code";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(resultsQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", Guid.Parse(studentId));
                        cmd.Parameters.AddWithValue("@semester_id", Guid.Parse(semesterId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int totalMarks = Convert.ToInt32(reader["total_marks"]);
                                decimal obtainedMarks = Convert.ToDecimal(reader["obtained_marks"]);
                                decimal percentage = (obtainedMarks / totalMarks) * 100;

                                SubjectResultData subjectData = new SubjectResultData
                                {
                                    SubjectCode = reader["subject_code"].ToString(),
                                    SubjectName = reader["subject_name"].ToString(),
                                    CreditHours = reader["credit_hours"] != DBNull.Value ? Convert.ToInt32(reader["credit_hours"]) : 3,
                                    TotalMarks = totalMarks,
                                    ObtainedMarks = obtainedMarks,
                                    Percentage = percentage,
                                    GPA = reader["gpa"] != DBNull.Value ? Convert.ToDecimal(reader["gpa"]) : 0,
                                    GradeLetter = reader["grade_letter"]?.ToString() ?? "F"
                                };
                                semData.Subjects.Add(subjectData);
                            }
                        }
                    }

                    // Calculate semester GPA and credit hours
                    if (semData.Subjects.Count > 0)
                    {
                        semData.SemesterCreditHours = semData.Subjects.Sum(s => s.CreditHours);
                        decimal totalGradePoints = semData.Subjects.Sum(s => s.GPA * s.CreditHours);
                        semData.SemesterGPA = semData.SemesterCreditHours > 0 ? 
                            totalGradePoints / semData.SemesterCreditHours : 0;
                        
                        transcript.Semesters.Add(semData);
                        transcript.TotalCreditHours = semData.SemesterCreditHours;
                        transcript.OverallCGPA = semData.SemesterGPA;
                    }
                }

                return transcript;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching semester transcript: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets list of semesters a student has results for
        /// </summary>
        public List<SemesterModel> GetStudentSemesters(string studentId)
        {
            List<SemesterModel> semesters = new List<SemesterModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT DISTINCT sem.id, sem.name, sem.start_date, sem.end_date, sem.is_current
                                   FROM enrollments e
                                   INNER JOIN semesters sem ON e.semester_id = sem.id
                                   INNER JOIN results r ON e.id = r.enrollment_id
                                   WHERE e.student_id = @student_id
                                   ORDER BY sem.start_date";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@student_id", Guid.Parse(studentId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                semesters.Add(new SemesterModel
                                {
                                    Id = reader["id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    StartDate = reader["start_date"] != DBNull.Value ? Convert.ToDateTime(reader["start_date"]) : DateTime.MinValue,
                                    EndDate = reader["end_date"] != DBNull.Value ? Convert.ToDateTime(reader["end_date"]) : DateTime.MinValue,
                                    IsCurrent = reader["is_current"] != DBNull.Value && Convert.ToBoolean(reader["is_current"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting student semesters: {ex.Message}", ex);
            }

            return semesters;
        }
    }
}
