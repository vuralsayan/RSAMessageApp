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
        //Data Source=Vural\SQLEXPRESS;Initial Catalog=DbRsaMessage;Integrated Security=True
        //Data Source=BTSTAJER08\MSSQLSERVER01;Initial Catalog=DbRsaMessage;User ID=vural; Password=vural123
        private static string connectionString = @"Data Source=Vural\SQLEXPRESS;Initial Catalog=DbRsaMessage;Integrated Security=True";

        public static SqlConnection CreateConnection()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            return connection;
        }

    }
}
