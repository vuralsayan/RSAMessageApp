using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; // SQL Server için gerekli kütüphane
using System.Security.Cryptography; // RSA için gerekli kütüphane

namespace RSAMessageApp
{
    public partial class Kayıt : Form
    {
        public Kayıt()
        {
            InitializeComponent();
        }

        // SQL Server bağlantısı 
        SqlConnection baglanti = new SqlConnection(@"Data Source=Vural\SQLEXPRESS;Initial Catalog=DbRsaMessage;Integrated Security=True");



        private void BtnRegister_Click(object sender, EventArgs e)
        {
            RegisterUser(TxtUsername.Text, TxtPassword.Text);    


            //Kullanıcı şifresini SHA-256 ile hashleyen fonksiyon
            void RegisterUser(string username, string password)
            {
                byte[] hashedPassword = HashPassword(password);

                string hashedPasswordString = Convert.ToBase64String(hashedPassword);

                MessageBox.Show(hashedPasswordString);

                //Veritabanına kayıt işlemi 
                SaveUserToDatabase(username, hashedPasswordString);
            }


            byte[] HashPassword(string password)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    //Girilen şifreyi byte dizisine çevirme
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                    return sha256.ComputeHash(passwordBytes);
                }
            }


            //Kullanıcıyı veritabanına kaydetme
            void SaveUserToDatabase(string username, string hashedPassword)
            {
                string connectionString = @"Data Source=Vural\SQLEXPRESS;Initial Catalog=DbRsaMessage;Integrated Security=True"; // Veritabanı bağlantı dizesi
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO TBLUSERS (Username, PasswordHash) VALUES (@Username, @HashedPassword)";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);
                        command.Parameters.AddWithValue("@HashedPassword", hashedPassword);

                        command.ExecuteNonQuery();
                    }
                }
            }

            MessageBox.Show("Kayıt Başarılı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);   


        }
    }
}


           