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
    public partial class Kullanicilar : Form
    {
        public Kullanicilar()
        {
            InitializeComponent();
        }


        private void Kullanicilar_Load(object sender, EventArgs e)
        {
            GetUserList();
        }

        public void GetUserList()
        {
            string query = "SELECT UserID as 'ID',Username as 'Kullanıcı Adı'FROM TBLUSERS";

            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
            dataGridView1.Columns["ID"].Width = 60; // "ID" sütununu 50 piksel genişliğinde ayarla
            dataGridView1.Columns["Kullanıcı Adı"].Width = 180;

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                int userID = GetUserIDFromSelectedRow(e);

                if (userID != -1)
                {
                    string userName = GetUsernameFromID(userID);

                    Mesaj existingMesaj = Application.OpenForms.OfType<Mesaj>().FirstOrDefault();

                    if (!string.IsNullOrEmpty(userName))
                    {
                        existingMesaj.TxtReceiverText = userName;
                        existingMesaj.Show();
                    }
                    else
                    {
                        MessageBox.Show("Kullanıcı adı bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Kullanıcı ID bulunamadı!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }
        }

        private int GetUserIDFromSelectedRow(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                int userID = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                return userID;
            }
            return -1;
        }

        private string GetUsernameFromID(int userID)
        {
            string userName = "";

            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                string query = "SELECT Username FROM TBLUSERS WHERE UserID=@UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userID);

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            userName = dr["Username"].ToString();
                        }
                    }
                }

                connection.Close();
            }

            return userName;
        }
    }
}
