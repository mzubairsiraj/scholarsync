using System;
using System.Collections.Generic;
using Npgsql;
using ScholarSync.Db_Connection;
using ScholarSync.Models;

namespace ScholarSync.Repositories
{
    public class ProgramRepository
    {
       
        public string AddProgram(ProgramModel program)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO programs (dept_id, name, duration_years, is_active) 
                                   VALUES (@dept_id, @name, @duration_years, true) 
                                   RETURNING id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dept_id", Guid.Parse(program.DeptId));
                        cmd.Parameters.AddWithValue("@name", program.Name);
                        cmd.Parameters.AddWithValue("@duration_years", program.DurationYears);

                        object result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding program: {ex.Message}", ex);
            }
        }

        
        public List<ProgramModel> GetAllPrograms()
        {
            List<ProgramModel> programs = new List<ProgramModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT p.id, p.dept_id, p.name, p.duration_years, p.is_active,
                                           d.name as dept_name
                                    FROM programs p
                                    INNER JOIN departments d ON p.dept_id = d.id
                                    WHERE p.is_active = true
                                    ORDER BY p.name";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                programs.Add(new ProgramModel
                                {
                                    Id = reader["id"].ToString(),
                                    DeptId = reader["dept_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    DurationYears = Convert.ToInt32(reader["duration_years"]),
                                    IsActive = Convert.ToBoolean(reader["is_active"]),
                                    DepartmentName = reader["dept_name"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching programs: {ex.Message}", ex);
            }

            return programs;
        }

        /// <summary>
        /// Gets programs by department
        /// </summary>
        public List<ProgramModel> GetProgramsByDepartment(string deptId)
        {
            List<ProgramModel> programs = new List<ProgramModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT p.id, p.dept_id, p.name, p.duration_years, p.is_active,
                                           d.name as dept_name
                                    FROM programs p
                                    INNER JOIN departments d ON p.dept_id = d.id
                                    WHERE p.dept_id = @dept_id AND p.is_active = true
                                    ORDER BY p.name";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@dept_id", Guid.Parse(deptId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                programs.Add(new ProgramModel
                                {
                                    Id = reader["id"].ToString(),
                                    DeptId = reader["dept_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    DurationYears = Convert.ToInt32(reader["duration_years"]),
                                    IsActive = Convert.ToBoolean(reader["is_active"]),
                                    DepartmentName = reader["dept_name"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching programs by department: {ex.Message}", ex);
            }

            return programs;
        }

        /// <summary>
        /// Updates an existing program
        /// </summary>
        public bool UpdateProgram(ProgramModel program)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE programs 
                                   SET dept_id = @dept_id, 
                                       name = @name, 
                                       duration_years = @duration_years
                                   WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.Parse(program.Id));
                        cmd.Parameters.AddWithValue("@dept_id", Guid.Parse(program.DeptId));
                        cmd.Parameters.AddWithValue("@name", program.Name);
                        cmd.Parameters.AddWithValue("@duration_years", program.DurationYears);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating program: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Deletes a program (soft delete - sets is_active to false)
        /// </summary>
        public bool DeleteProgram(string programId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE programs SET is_active = false WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.Parse(programId));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting program: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Gets a program by ID
        /// </summary>
        public ProgramModel GetProgramById(string programId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT p.id, p.dept_id, p.name, p.duration_years, p.is_active,
                                           d.name as dept_name
                                    FROM programs p
                                    INNER JOIN departments d ON p.dept_id = d.id
                                    WHERE p.id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.Parse(programId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new ProgramModel
                                {
                                    Id = reader["id"].ToString(),
                                    DeptId = reader["dept_id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    DurationYears = Convert.ToInt32(reader["duration_years"]),
                                    IsActive = Convert.ToBoolean(reader["is_active"]),
                                    DepartmentName = reader["dept_name"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error fetching program: {ex.Message}", ex);
            }

            return null;
        }
    }
}
