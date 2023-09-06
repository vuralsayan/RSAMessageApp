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
using System.Security.Cryptography;

namespace RSAMessageApp
{
    public partial class MesajDetay : Form
    {
        public MesajDetay()
        {
            InitializeComponent();
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowOnly;
        }

        public Mesaj MesajFormReference { get; set; }

        public string message;
        public string senderName;
        public string date;
        public int messageID;
        public string showUsername;

        private void MesajDetay_Load(object sender, EventArgs e)
        {
            textBox1.Text = senderName;
            textBox2.Text = date;
            richTextBox1.Text = message;
            SetMessageAsRead(messageID);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            int messageIDToDelete = messageID;

            string deleteQuery = "DELETE FROM TBLMESSAGES WHERE MessageID = @MessageID";

            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                {
                    command.Parameters.AddWithValue("@MessageID", messageIDToDelete);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Mesaj veritabanından başarıyla silindi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Mesaj silinirken bir hata oluştu.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    if (MesajFormReference != null)
                    {
                        MesajFormReference.ShowMessages();
                    }
                }
            }
        }

        private void BtnAnswer_Click(object sender, EventArgs e)
        {
            string messageSenderName = senderName;

            // Öncelikle açık olan Mesaj formunu bulun
            Mesaj existingMesaj = Application.OpenForms.OfType<Mesaj>().FirstOrDefault();

            if (existingMesaj != null)
            {
                existingMesaj.TxtReceiverText = messageSenderName; // Kullanıcı adını güncelleyin
                existingMesaj.Show(); // Mevcut Mesaj formunu gösterin
            }
            else
            {
                // Mesaj formu açık değilse, yeni bir tane oluşturun
                Mesaj msj = new Mesaj();
                msj.showUsername = messageSenderName;
                msj.Show();
            }

            this.Close();
        }

        // Veritabanında ilgili mesajın "ReadStatus" değerini 1 yapar
        private void SetMessageAsRead(int messageID)
        {
            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                string query = "UPDATE TBLMESSAGES SET ReadStatus = 1 WHERE MessageID = @MessageID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MessageID", messageID);

                    command.ExecuteNonQuery();
                }

                connection.Close();
            }
        }

        private void MesajDetay_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(MesajFormReference != null)
            {
                MesajFormReference.ShowMessages();
            }
        }
    }
}
