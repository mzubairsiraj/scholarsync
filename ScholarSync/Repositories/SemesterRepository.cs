using System;
using System.Collections.Generic;
using Npgsql;
using ScholarSync.Db_Connection;
using ScholarSync.Models;

namespace ScholarSync.Repositories
{
    /// <summary>
    /// Repository for managing semesters
    /// Handles all database operations for semesters table
    /// </summary>
    public class SemesterRepository
    {
        /// <summary>
        /// Gets all semesters
        /// </summary>
        public List<SemesterModel> GetAllSemesters()
        {
            List<SemesterModel> semesters = new List<SemesterModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT id, name, start_date, end_date, is_current
                                   FROM semesters
                                   ORDER BY start_date DESC";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                semesters.Add(new SemesterModel
                                {
                                    Id = reader["id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    StartDate = Convert.ToDateTime(reader["start_date"]),
                                    EndDate = Convert.ToDateTime(reader["end_date"]),
                                    IsCurrent = Convert.ToBoolean(reader["is_current"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting semesters: {ex.Message}", ex);
            }

            return semesters;
        }

        /// <summary>
        /// Gets the current semester
        /// </summary>
        public SemesterModel GetCurrentSemester()
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT id, name, start_date, end_date, is_current
                                   FROM semesters
                                   WHERE is_current = true
                                   LIMIT 1";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SemesterModel
                                {
                                    Id = reader["id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    StartDate = Convert.ToDateTime(reader["start_date"]),
                                    EndDate = Convert.ToDateTime(reader["end_date"]),
                                    IsCurrent = Convert.ToBoolean(reader["is_current"])
                                };
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting current semester: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets semester by ID
        /// </summary>
        public SemesterModel GetSemesterById(string semesterId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT id, name, start_date, end_date, is_current
                                   FROM semesters
                                   WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.Parse(semesterId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new SemesterModel
                                {
                                    Id = reader["id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    StartDate = Convert.ToDateTime(reader["start_date"]),
                                    EndDate = Convert.ToDateTime(reader["end_date"]),
                                    IsCurrent = Convert.ToBoolean(reader["is_current"])
                                };
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting semester: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Adds a new semester
        /// </summary>
        public string AddSemester(SemesterModel semester)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    // If this is set as current, unset all others first
                    if (semester.IsCurrent)
                    {
                        string unsetQuery = "UPDATE semesters SET is_current = false WHERE is_current = true";
                        using (NpgsqlCommand unsetCmd = new NpgsqlCommand(unsetQuery, conn))
                        {
                            unsetCmd.ExecuteNonQuery();
                        }
                    }

                    string query = @"INSERT INTO semesters (name, start_date, end_date, is_current)
                                   VALUES (@name, @start_date, @end_date, @is_current)
                                   RETURNING id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", semester.Name);
                        cmd.Parameters.AddWithValue("@start_date", semester.StartDate);
                        cmd.Parameters.AddWithValue("@end_date", semester.EndDate);
                        cmd.Parameters.AddWithValue("@is_current", semester.IsCurrent);

                        object result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding semester: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates an existing semester
        /// </summary>
        public bool UpdateSemester(SemesterModel semester)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    // If this is set as current, unset all others first
                    if (semester.IsCurrent)
                    {
                        string unsetQuery = "UPDATE semesters SET is_current = false WHERE is_current = true AND id != @id";
                        using (NpgsqlCommand unsetCmd = new NpgsqlCommand(unsetQuery, conn))
                        {
                            unsetCmd.Parameters.AddWithValue("@id", Guid.Parse(semester.Id));
                            unsetCmd.ExecuteNonQuery();
                        }
                    }

                    string query = @"UPDATE semesters
                                   SET name = @name,
                                       start_date = @start_date,
                                       end_date = @end_date,
                                       is_current = @is_current
                                   WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.Parse(semester.Id));
                        cmd.Parameters.AddWithValue("@name", semester.Name);
                        cmd.Parameters.AddWithValue("@start_date", semester.StartDate);
                        cmd.Parameters.AddWithValue("@end_date", semester.EndDate);
                        cmd.Parameters.AddWithValue("@is_current", semester.IsCurrent);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating semester: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Sets a semester as the current semester (unsets all others)
        /// </summary>
        public bool SetCurrentSemester(string semesterId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    using (var transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            // Unset all current semesters
                            string unsetQuery = "UPDATE semesters SET is_current = false WHERE is_current = true";
                            using (NpgsqlCommand unsetCmd = new NpgsqlCommand(unsetQuery, conn, transaction))
                            {
                                unsetCmd.ExecuteNonQuery();
                            }

                           
                            string setQuery = "UPDATE semesters SET is_current = true WHERE id = @id";
                            using (NpgsqlCommand setCmd = new NpgsqlCommand(setQuery, conn, transaction))
                            {
                                setCmd.Parameters.AddWithValue("@id", Guid.Parse(semesterId));
                                int rowsAffected = setCmd.ExecuteNonQuery();
                                
                                transaction.Commit();
                                return rowsAffected > 0;
                            }
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error setting current semester: {ex.Message}", ex);
            }
        }

        
        public bool DeleteSemester(string semesterId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = "DELETE FROM semesters WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.Parse(semesterId));
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting semester: {ex.Message}", ex);
            }
        }
    }
}
