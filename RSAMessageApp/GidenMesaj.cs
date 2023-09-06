using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace RSAMessageApp
{
    public partial class GidenMesaj : Form
    {
        public GidenMesaj()
        {
            InitializeComponent();
        }

        public string senderName { get; set; }

        private void GidenMesaj_Load(object sender, EventArgs e)
        {
            int userID = GetUserIDFromUsername(senderName);
            DataTable dt =  GetMessagesFromDatabase(userID);
            dataGridView1.DataSource = dt;
            dataGridView1.Columns["ID"].Width = 50; // "ID" sütununu 50 piksel genişliğinde ayarla
            dataGridView1.Columns["Alıcı"].Width = 170; // "ID" sütununu 50 piksel genişliğinde ayarla
            dataGridView1.Columns["Başlık"].Width = 200; // "ID" sütununu 50 piksel genişliğinde ayarla
            dataGridView1.Columns["Tarih"].Width = 170; // "ID" sütununu 50 piksel genişliğinde ayarla

        }
        private DataTable GetMessagesFromDatabase(int userID)
        {
            string query = "SELECT " +
                "MessageID AS 'ID', " +
                "TBLUSERS_Receiver.Username AS 'Alıcı'," +
                "Title AS 'Başlık'," +
                "FORMAT(TBLMESSAGES.Timestamp, 'dd-MM-yyyy HH:mm:ss') AS 'Tarih' " +
                "FROM TBLMESSAGES " +
                "INNER JOIN TBLUSERS AS TBLUSERS_Sender ON TBLMESSAGES.SenderID = TBLUSERS_Sender.UserID " +
                "INNER JOIN TBLUSERS AS TBLUSERS_Receiver ON TBLMESSAGES.ReceiverID = TBLUSERS_Receiver.UserID " +
                "WHERE TBLUSERS_Sender.UserID = @UserID";

            SqlDataAdapter da = new SqlDataAdapter(query, Connection.CreateConnection());
            da.SelectCommand.Parameters.AddWithValue("@UserID", userID);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        private int GetUserIDFromUsername(string username)
        {
            int userID = -1;

            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                string query = "SELECT UserID From TBLUSERS WHERE Username=@Username";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            userID = (int)dr["UserID"];
                        }
                    }
                }

                connection.Close();
            }

            return userID;
        }

    }
}
