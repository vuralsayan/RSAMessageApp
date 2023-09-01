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

        void GelenKutusu()
        {
            string query = $"SELECT MessageID,TBLUSERS_Sender.Username AS SenderUsername, TBLUSERS_Receiver.Username AS ReceiverUsername, EncryptedMessage, FORMAT(TBLMESSAGES.Timestamp, 'yyyy-MM-dd HH:mm:ss') AS FormattedTimestamp FROM TBLMESSAGES INNER JOIN TBLUSERS AS TBLUSERS_Sender ON TBLMESSAGES.SenderID = TBLUSERS_Sender.UserID INNER JOIN TBLUSERS AS TBLUSERS_Receiver ON TBLMESSAGES.ReceiverID = TBLUSERS_Receiver.UserID WHERE TBLUSERS_Receiver.Username = '{showUsername}'";
            SqlDataAdapter da = new SqlDataAdapter(query, Connection.CreateConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView1.DataSource = dt;

        }

        void GidenKutusu()
        {
            string query = $"SELECT MessageID,TBLUSERS_Sender.Username AS SenderUsername, TBLUSERS_Receiver.Username AS ReceiverUsername, EncryptedMessage, FORMAT(TBLMESSAGES.Timestamp, 'yyyy-MM-dd HH:mm:ss') AS FormattedTimestamp FROM TBLMESSAGES INNER JOIN TBLUSERS AS TBLUSERS_Sender ON TBLMESSAGES.SenderID = TBLUSERS_Sender.UserID INNER JOIN TBLUSERS AS TBLUSERS_Receiver ON TBLMESSAGES.ReceiverID = TBLUSERS_Receiver.UserID WHERE TBLUSERS_Sender.Username = '{showUsername}';";
            SqlDataAdapter da = new SqlDataAdapter(query, Connection.CreateConnection());
            DataTable dt = new DataTable();
            da.Fill(dt);
            dataGridView2.DataSource = dt;
        }


        private void Mesaj_Load(object sender, EventArgs e)
        {
            LblUsername.Text = showUsername;
            //GelenKutusu();
            //GidenKutusu();

            string receiverPrivateKey = GetPrivateKeyByUsername(showUsername);
            string senderPublicKey = GetPublicKeyByUsername(showUsername);

            LoadDecryptedMessagesToDataGridView(dataGridView1, "Inbox", receiverPrivateKey, senderPublicKey);
            LoadDecryptedMessagesToDataGridView(dataGridView2, "Outbox", receiverPrivateKey, senderPublicKey);
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



        // Gönderenin mesajı şifrelemesi ve imzalaması
        public string EncryptAndSignMessage(string message, string senderPrivateKey)
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
            try
            {
                string senderName = LblUsername.Text;
                string receiverName = TxtReceiver.Text;
                string message = richTextBox1.Text;

                // Gönderen ve alıcının keyleri alma
                string senderPrivateKey = GetPrivateKeyByUsername(senderName);
                string senderPublicKey = GetPublicKeyByUsername(senderName);
                string receiverPublicKey = GetPublicKeyByUsername(receiverName);
                string receiverPrivateKey = GetPrivateKeyByUsername(receiverName);

                // Mesajı şifrele ve imzala
                string signedMessage = EncryptAndSignMessage(message, senderPrivateKey);

                // Gönderen ve alıcının ID'lerini al
                int senderID = GetUserIDByUsername(senderName);
                int receiverID = GetUserIDByUsername(receiverName);

                // Şifrelenmiş ve imzalanmış mesajı veritabanına kaydet
                SaveEncryptedMessageToDatabase(senderID, receiverID, signedMessage);

                MessageBox.Show("Mesaj başarıyla gönderildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (CryptographicException ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata Oluştu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        public List<string> GetEncryptedMessagesFromDatabase(string boxType)
        {
            List<string> encryptedMessages = new List<string>();

            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                string query = "";
                if (boxType == "Inbox") //Gelen Kutusu
                {
                    query = "SELECT EncryptedMessage FROM TBLMESSAGES WHERE ReceiverID = @ReceiverID";
                }
                else if (boxType == "Outbox")   //Giden kutusu
                {
                    query = "SELECT EncryptedMessage FROM TBLMESSAGES WHERE SenderID = @SenderID";
                }

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Alıcı veya gönderici ID'sini parametre olarak ekleyin
                    if (boxType == "Inbox")
                    {
                        command.Parameters.AddWithValue("@ReceiverID", GetUserIDByUsername(showUsername)); // showUsername, mevcut kullanıcının adını içerir
                    }
                    else if (boxType == "Outbox")
                    {
                        command.Parameters.AddWithValue("@SenderID", GetUserIDByUsername(showUsername)); // showUsername, mevcut kullanıcının adını içerir
                    }

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string encryptedMessage = dr["EncryptedMessage"].ToString();
                            encryptedMessages.Add(encryptedMessage);
                        }
                    }
                }

                connection.Close();
            }

            return encryptedMessages;
        }




        private void LoadDecryptedMessagesToDataGridView(DataGridView dataGridView, string boxType, string receiverPrivateKey, string senderPublicKey)
        {
            // Şifreli mesajları alın ve çözün, her mesajı bir koleksiyon içinde saklayın
            List<string> encryptedMessages = GetEncryptedMessagesFromDatabase(boxType);
            List<string> decryptedMessages = new List<string>();

            foreach (string encryptedMessage in encryptedMessages)
            {
                string decryptedMessage = DecryptAndVerifyMessage(encryptedMessage, receiverPrivateKey, senderPublicKey);
                decryptedMessages.Add(decryptedMessage);
            }

            // DataGridView için bir BindingSource oluşturun ve verileri yükleyin
            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = decryptedMessages;

            // DataGridView'ı ayarlayın
            dataGridView.DataSource = bindingSource;
        }



        private void SaveEncryptedMessageToDatabase(int senderID, int receiverID, string encryptedMessage)
        {
            using (SqlConnection connection = Connection.CreateConnection())
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO TBLMESSAGES (SenderID, ReceiverID, EncryptedMessage) VALUES (@SenderID, @ReceiverID, @EncryptedMessage)", connection))
                {
                    command.Parameters.AddWithValue("@SenderID", senderID);
                    command.Parameters.AddWithValue("@ReceiverID", receiverID);
                    command.Parameters.AddWithValue("@EncryptedMessage", encryptedMessage);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


    }
}


