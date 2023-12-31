﻿using System;
using System.Data.SqlClient; // SQL Server için gerekli kütüphane
using System.Security.Cryptography; // RSA için gerekli kütüphane
using System.Text;
using System.Windows.Forms;

namespace RSAMessageApp
{
    public partial class Kayıt : Form
    {
        private RSACryptoServiceProvider rsa; // Anahtar çiftini saklamak için
        public Kayıt()
        {
            InitializeComponent();
            rsa = new RSACryptoServiceProvider(); // Anahtar çiftini oluştur
        }


        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string username = TxtUsername.Text;
            string password = TxtPassword.Text;

            RegisterUser(username, password);

        }

        //Kullanıcı şifresini SHA-256 ile hashleyen fonksiyon
        void RegisterUser(string username, string password)
        {
            byte[] hashedPassword = HashPassword(password);
            string hashedPasswordString = Convert.ToBase64String(hashedPassword);

            string publicKey = rsa.ToXmlString(false); // Public anahtar
            string privateKey = rsa.ToXmlString(true); // Private anahtar

            SaveUserToDatabase(username, hashedPasswordString, publicKey, privateKey);

            MessageBox.Show("Kayıt başarılı", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
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


        void SaveUserToDatabase(string username, string hashedPassword, string publicKey, string privateKey)
        {
            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();
                string query = "INSERT INTO TBLUSERS (Username, PasswordHash, PublicKey, PrivateKey) VALUES (@Username, @HashedPassword, @PublicKey, @PrivateKey)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@HashedPassword", hashedPassword);
                    command.Parameters.AddWithValue("@PublicKey", publicKey);
                    command.Parameters.AddWithValue("@PrivateKey", privateKey);

                    command.ExecuteNonQuery();
                }

            }

        }
    }
}


