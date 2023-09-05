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
    public partial class Giris : Form
    {
        public Giris()
        {
            InitializeComponent();
        }

        private void BtnRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Kayıt kyt = new Kayıt();
            kyt.Show();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {

            if (AuthenticateUser(TxtUsername.Text, TxtPassword.Text) == true)
            {
                Mesaj msj = new Mesaj();
                msj.showUsername = TxtUsername.Text;
                msj.Show();
                this.Hide();
            }

            // Kullanıcının girişini doğrulama
            bool AuthenticateUser(string username, string password)
            {
                // Kullanıcının hash'ini veritabanından al
                string storedHashedPassword = GetHashedPasswordByUsername(username);

                if (string.IsNullOrEmpty(storedHashedPassword))
                {
                    MessageBox.Show("Kullanıcının şifresi kaydedilmemiş.","Bilgi",MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                byte[] inputHashedPassword = HashPassword(password);
                string inputHashedPasswordString = Convert.ToBase64String(inputHashedPassword);

                return storedHashedPassword == inputHashedPasswordString;
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

            // Kullanıcının hash'ini veritabanından al
            string GetHashedPasswordByUsername(string username)
            {
                string hashedPassword = "";

                using (SqlConnection connection = Connection.CreateConnection())
                {
                    connection.Open();

                    string query = "SELECT PasswordHash FROM TBLUSERS WHERE Username = @Username";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Username", username);

                        // SqlDataReader kullanarak şifreyi alın
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                hashedPassword = reader.GetString(0);
                            }
                        }
                    }
                }

                return hashedPassword;
            }
        }
        
    }
}
