using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;

namespace RSAMessageApp
{
    public partial class Mesaj : Form
    {
        public Mesaj()
        {
            InitializeComponent();
        }

        public string showUsername;

        private void Mesaj_Load(object sender, EventArgs e)
        {
            LblUsername.Text = $"Hoşgeldin {showUsername}";
            ShowMessages();
        }

        void ShowMessages()
        {
            DataTable dt = GetMessagesFromDatabase(GetUserIDByUsername(showUsername));
            dataGridView1.DataSource = dt;
        }

        public string GetPrivateKeyByUsername(string username)
        {
            string privateKey = "";


            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT PrivateKey FROM TBLUSERS WHERE Username=@Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            privateKey = dr[0].ToString();
                        }
                    }
                }
                connection.Close();

            }

            return privateKey;
        }

        public string GetPublicKeyByUsername(string username)
        {
            string publicKey = "";

            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT PublicKey FROM TBLUSERS WHERE Username=@Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            publicKey = dr[0].ToString();
                        }
                    }
                }
                connection.Close();

            }

            return publicKey;
        }

        //Veritabanından mesaj başlıklarını çekme
        private DataTable GetMessagesFromDatabase(int userID)
        {
            string query = "SELECT " +
                "MessageID AS 'MessageID', " +
                "TBLUSERS_Sender.Username AS 'Gönderen', " +
                "TBLUSERS_Receiver.Username AS 'Alıcı'," +
                "Title AS 'Başlık'," +
                "FORMAT(TBLMESSAGES.Timestamp, 'dd-MM-yyyy HH:mm:ss') AS 'Tarih' " +
                "FROM TBLMESSAGES " +
                "INNER JOIN TBLUSERS AS TBLUSERS_Sender ON TBLMESSAGES.SenderID = TBLUSERS_Sender.UserID " +
                "INNER JOIN TBLUSERS AS TBLUSERS_Receiver ON TBLMESSAGES.ReceiverID = TBLUSERS_Receiver.UserID " +
                "WHERE TBLUSERS_Receiver.UserID = @UserID";

            SqlDataAdapter da = new SqlDataAdapter(query, Connection.CreateConnection());
            da.SelectCommand.Parameters.AddWithValue("@UserID", userID);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return dt;
        }

        // Gönderenin mesajı şifrelemesi ve imzalaması
        public string EncryptAndSignMessage(string message, string senderPrivateKey, string receiverPublicKey)
        {
            using (RSACryptoServiceProvider senderRsa = new RSACryptoServiceProvider())
            {
                senderRsa.FromXmlString(senderPrivateKey);

                // Mesajı şifrele
                byte[] encryptedBytes = senderRsa.Encrypt(Encoding.UTF8.GetBytes(message), true);

                // Şifrelenmiş mesajı base64 formatında kodla
                string encryptedMessage = Convert.ToBase64String(encryptedBytes);

                // Gönderenin public keyini al
                string senderPublicKey = senderRsa.ToXmlString(false);

                // Mesajı ve gönderenin public keyini birleştir
                string signedMessage = $"{encryptedMessage}|{senderPublicKey}";

                return signedMessage;
            }
        }


        //Alıcının mesajı çözüp doğrulaması
        public string DecryptAndVerifyMessage(string signedMessage, string receiverPrivateKey, string senderPublicKey)
        {
            using (RSACryptoServiceProvider receiverRsa = new RSACryptoServiceProvider())
            {
                receiverRsa.FromXmlString(receiverPrivateKey);

                // İmzalı mesajı ayır
                string[] parts = signedMessage.Split('|');
                string encryptedMessage = parts[0];
                string receivedSenderPublicKey = parts[1];

                // Alıcının kendi private keyi ile mesajı çöz
                byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);
                byte[] decryptedBytes = receiverRsa.Decrypt(encryptedBytes, true);

                // Mesajı döndür
                string decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);

                // Gönderenin public keyi ile imzayı doğrula
                bool isSignatureValid = VerifySignature(decryptedMessage, receivedSenderPublicKey, senderPublicKey);

                if (isSignatureValid)
                {
                    return decryptedMessage;
                }
                else
                {
                    throw new Exception("İmza doğrulama hatası: Gönderen kimliği doğrulanamadı.");
                }
            }
        }


        //İmzanın doğrulanması
        public bool VerifySignature(string message, string receivedSenderPublicKey, string originalSenderPublicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(receivedSenderPublicKey);

                // Orjinal gönderenin public keyini base64'den byte dizisine dönüştür
                byte[] originalPublicKeyBytes = Convert.FromBase64String(originalSenderPublicKey);

                // Veriyi SHA-256 ile imzala ve imzayı doğrula
                bool result = rsa.VerifyData(Encoding.UTF8.GetBytes(message), new SHA256CryptoServiceProvider(), originalPublicKeyBytes);

                return result;
            }
        }


        public int GetUserIDByUsername(string username)
        {
            int userID = -1;

            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("SELECT UserID FROM TBLUSERS WHERE Username=@Username", connection))
                {
                    command.Parameters.AddWithValue("@Username", username);

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            userID = Convert.ToInt32(dr["UserID"]);
                        }
                    }
                }
                connection.Close();
            }

            return userID;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            string senderName = showUsername;
            string receiverName = TxtReceiver.Text;
            string message = richTextBox1.Text;

            // Gönderen ve alıcının keyleri alma
            string senderPrivateKey = GetPrivateKeyByUsername(senderName);
            string senderPublicKey = GetPublicKeyByUsername(senderName);
            string receiverPublicKey = GetPublicKeyByUsername(receiverName);
            string receiverPrivateKey = GetPrivateKeyByUsername(receiverName);

            // Mesajı şifrele ve imzala
            string signedMessage = EncryptAndSignMessage(message, senderPrivateKey, receiverPublicKey);

            // Gönderen ve alıcının ID'lerini al
            int senderID = GetUserIDByUsername(senderName);
            int receiverID = GetUserIDByUsername(receiverName);

            // Şifrelenmiş ve imzalanmış mesajı veritabanına kaydet
            SaveEncryptedMessageToDatabase(senderID, receiverID, signedMessage);

            MessageBox.Show("Mesaj başarıyla gönderildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            ShowMessages();
        }

        private void SaveEncryptedMessageToDatabase(int senderID, int receiverID, string encryptedMessage)
        {
            using (SqlConnection connection = Connection.CreateConnection())
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO TBLMESSAGES (SenderID, ReceiverID, Title, EncryptedMessage) VALUES (@SenderID, @ReceiverID, @Title, @EncryptedMessage)", connection))
                {
                    command.Parameters.AddWithValue("@SenderID", senderID);
                    command.Parameters.AddWithValue("@ReceiverID", receiverID);
                    command.Parameters.AddWithValue("@Title", TxtTitle.Text);
                    command.Parameters.AddWithValue("@EncryptedMessage", encryptedMessage);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            TxtReceiver.Clear();
            TxtTitle.Clear();
            richTextBox1.Clear();
            TxtReceiver.Focus();
        }

        private int GetMessageIDFromSelectedRow(DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
                int messageID = Convert.ToInt32(selectedRow.Cells["MessageID"].Value);
                return messageID;
            }
            return -1; 
        }

        private string GetEncryptedMessageByMessageID(int messageID)
        {
            string encryptedMessage = null;

            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                string query = "SELECT EncryptedMessage FROM TBLMESSAGES WHERE MessageID = @MessageID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MessageID", messageID);

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            encryptedMessage = dr["EncryptedMessage"].ToString();
                        }
                    }
                }

                connection.Close();
            }

            return encryptedMessage;
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                int messageID = GetMessageIDFromSelectedRow(e);

                if (messageID != -1)
                {
                    string encryptedMessage = GetEncryptedMessageByMessageID(messageID);

                    if (!string.IsNullOrEmpty(encryptedMessage))
                    {
                        MessageBox.Show(encryptedMessage);
                    }
                    else
                    {
                        MessageBox.Show("Mesaj bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    MessageBox.Show("MessageID alınamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
    }
}

