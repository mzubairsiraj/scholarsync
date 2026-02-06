using System;
using System.Collections.Generic;
using Npgsql;
using ScholarSync.Db_Connection;
using ScholarSync.Models;

namespace ScholarSync.Repositories
{
    
    public class DepartmentRepository
    {
        public List<DepartmentModel> GetAllDepartments()
        {
            List<DepartmentModel> departments = new List<DepartmentModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT id, name, code, head_of_department_id, is_active
                                   FROM departments
                                   ORDER BY name";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                departments.Add(new DepartmentModel
                                {
                                    Id = reader["id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Code = reader["code"].ToString(),
                                    HeadOfDepartmentId = reader["head_of_department_id"] != DBNull.Value ? reader["head_of_department_id"].ToString() : null,
                                    IsActive = Convert.ToBoolean(reader["is_active"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting departments: {ex.Message}", ex);
            }

            return departments;
        }

        /// <summary>
        /// Gets active departments only
        /// </summary>
        /// 
       
        public List<DepartmentModel> GetActiveDepartments()
        {
            List<DepartmentModel> departments = new List<DepartmentModel>();

            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT id, name, code, head_of_department_id, is_active
                                   FROM departments
                                   WHERE is_active = true
                                   ORDER BY name";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                departments.Add(new DepartmentModel
                                {
                                    Id = reader["id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Code = reader["code"].ToString(),
                                    HeadOfDepartmentId = reader["head_of_department_id"] != DBNull.Value ? reader["head_of_department_id"].ToString() : null,
                                    IsActive = Convert.ToBoolean(reader["is_active"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting active departments: {ex.Message}", ex);
            }

            return departments;
        }

       
        public string AddDepartment(DepartmentModel department)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"INSERT INTO departments (name, code, head_of_department_id, is_active)
                                   VALUES (@name, @code, @head_of_department_id, @is_active)
                                   RETURNING id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@name", department.Name);
                        cmd.Parameters.AddWithValue("@code", department.Code);
                        cmd.Parameters.AddWithValue("@head_of_department_id", !string.IsNullOrEmpty(department.HeadOfDepartmentId) ? (object)Guid.Parse(department.HeadOfDepartmentId) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@is_active", department.IsActive);

                        object result = cmd.ExecuteScalar();
                        return result?.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding department: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Updates an existing department
        /// </summary>
        public bool UpdateDepartment(DepartmentModel department)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"UPDATE departments
                                   SET name = @name,
                                       code = @code,
                                       head_of_department_id = @head_of_department_id,
                                       is_active = @is_active
                                   WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.Parse(department.Id));
                        cmd.Parameters.AddWithValue("@name", department.Name);
                        cmd.Parameters.AddWithValue("@code", department.Code);
                        cmd.Parameters.AddWithValue("@head_of_department_id", !string.IsNullOrEmpty(department.HeadOfDepartmentId) ? (object)Guid.Parse(department.HeadOfDepartmentId) : DBNull.Value);
                        cmd.Parameters.AddWithValue("@is_active", department.IsActive);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating department: {ex.Message}", ex);
            }
        }

        public DepartmentModel GetDepartmentById(string departmentId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                    string query = @"SELECT id, name, code, head_of_department_id, is_active
                                   FROM departments
                                   WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.Parse(departmentId));

                        using (NpgsqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new DepartmentModel
                                {
                                    Id = reader["id"].ToString(),
                                    Name = reader["name"].ToString(),
                                    Code = reader["code"].ToString(),
                                    HeadOfDepartmentId = reader["head_of_department_id"] != DBNull.Value ? reader["head_of_department_id"].ToString() : null,
                                    IsActive = Convert.ToBoolean(reader["is_active"])
                                };
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error getting department: {ex.Message}", ex);
            }
        }

       
        public bool DeleteDepartment(string departmentId)
        {
            try
            {
                using (NpgsqlConnection conn = DbConnector.GetConnection())
                {
                    conn.Open();

                   
                    string query = @"UPDATE departments
                                   SET is_active = false
                                   WHERE id = @id";

                    using (NpgsqlCommand cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", Guid.Parse(departmentId));
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting department: {ex.Message}", ex);
            }
        }
    }
}
