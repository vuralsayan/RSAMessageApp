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
        private static string connectionString = @"Data Source=Vural\SQLEXPRESS;Initial Catalog=DbRsaMessage;Integrated Security=True";

        public static SqlConnection CreateConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }

    }
}
