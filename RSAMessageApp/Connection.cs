using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace RSAMessageApp
{
    public class Connection
    {
        private static string connectionString = @"Data Source=BTSTAJER08\MSSQLSERVER01;Initial Catalog=DbRsaMessage;User ID=vural; Password=vural123";

        public static SqlConnection CreateConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }

    }
}
