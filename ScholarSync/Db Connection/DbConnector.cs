using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace ScholarSync.Db_Connection
{
    internal static class DbConnector
    {
        public static NpgsqlConnection GetConnection()
        {
            string connectionString = ScholarSync.Commons.ConfigurationConstants.ConnectionString;
            return new NpgsqlConnection(connectionString);
        }
    }
}
