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

        private Timer timer;
        public Mesaj()
        {
            InitializeComponent();
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            timer = new Timer();
            timer.Interval = 5000;          //her 5 saniyede
            timer.Tick += Timer_Tick;
        }

        private async void Timer_Tick(object sender, EventArgs e)
        {
            string username = showUsername;
            int userID = GetUserIDByUsername(username);

            DataTable messages = await FetchDataFromDatabaseAsync(userID); // userID'yi kendi kullanımınıza göre ayarlayın
            dataGridView1.DataSource = messages;
        }

        private void StartFetchingData()
        {
            // Timer'ı başlatmak için bu metodu kullanabilirsiniz
            timer.Start();
        }

        private void StopFetchingData()
        {
            // Timer'ı durdurmak için bu metodu kullanabilirsiniz
            timer.Stop();
        }

       


        private async Task<DataTable> FetchDataFromDatabaseAsync(int userID)
        {
            DataTable dt = new DataTable();
            try
            {
                string query = "SELECT " +
                    "MessageID AS 'ID', " +
                    "TBLUSERS_Sender.Username AS 'Gönderen', " +
                    "TBLUSERS_Receiver.Username AS 'Alıcı'," +
                    "Title AS 'Başlık'," +
                    "FORMAT(TBLMESSAGES.Timestamp, 'dd-MM-yyyy HH:mm:ss') AS 'Tarih' ," +
                    "ReadStatus AS 'Okundu' " +
                    "FROM TBLMESSAGES " +
                    "INNER JOIN TBLUSERS AS TBLUSERS_Sender ON TBLMESSAGES.SenderID = TBLUSERS_Sender.UserID " +
                    "INNER JOIN TBLUSERS AS TBLUSERS_Receiver ON TBLMESSAGES.ReceiverID = TBLUSERS_Receiver.UserID " +
                    "WHERE TBLUSERS_Receiver.UserID = @UserID";

                SqlDataAdapter da = new SqlDataAdapter(query, Connection.CreateConnection());
                da.SelectCommand.Parameters.AddWithValue("@UserID", userID);

                await Task.Run(() =>
                {
                    da.Fill(dt);
                });
            }
            catch (Exception ex)
            {
                // Hata yönetimi burada yapılabilir
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dt;
        }


        public string showUsername { get; set; }
        public string TxtReceiverText
        {
            get { return TxtReceiver.Text; }
            set { TxtReceiver.Text = value; }
        }


        private void Mesaj_Load(object sender, EventArgs e)
        {
            LblUsername.Text = $"Hoşgeldin {showUsername}";
            StartFetchingData();
        }

        public async Task ShowMessagesAsync()
        {
            try
            {
                int userID = GetUserIDByUsername(showUsername);
                DataTable dt = await FetchDataFromDatabaseAsync(userID);

                // Verileri DataGridView'e bağlayın
                dataGridView1.DataSource = dt;
                dataGridView1.Columns["ID"].Width = 50;
                dataGridView1.Columns["Gönderen"].Width = 130;
                dataGridView1.Columns["Alıcı"].Width = 140;
                dataGridView1.Columns["Başlık"].Width = 172;
                dataGridView1.Columns["Tarih"].Width = 170;
                dataGridView1.Columns["Okundu"].Width = 70;
            }
            catch (Exception ex)
            {
                // Hata yönetimi burada yapılabilir
                MessageBox.Show($"Hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void mesajTimer_Tick(object sender, EventArgs e)
        {
            ShowMessagesAsync();
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

        //Veritabanından mesajları çekme
        //private DataTable GetMessagesFromDatabase(int userID)
        //{
        //    string query = "SELECT " +
        //        "MessageID AS 'ID', " +
        //        "TBLUSERS_Sender.Username AS 'Gönderen', " +
        //        "TBLUSERS_Receiver.Username AS 'Alıcı'," +
        //        "Title AS 'Başlık'," +
        //        "FORMAT(TBLMESSAGES.Timestamp, 'dd-MM-yyyy HH:mm:ss') AS 'Tarih' ," +
        //        "ReadStatus AS 'Okundu' " +
        //        "FROM TBLMESSAGES " +
        //        "INNER JOIN TBLUSERS AS TBLUSERS_Sender ON TBLMESSAGES.SenderID = TBLUSERS_Sender.UserID " +
        //        "INNER JOIN TBLUSERS AS TBLUSERS_Receiver ON TBLMESSAGES.ReceiverID = TBLUSERS_Receiver.UserID " +
        //        "WHERE TBLUSERS_Receiver.UserID = @UserID";

        //    SqlDataAdapter da = new SqlDataAdapter(query, Connection.CreateConnection());
        //    da.SelectCommand.Parameters.AddWithValue("@UserID", userID);
        //    DataTable dt = new DataTable();
        //    da.Fill(dt);
        //    return dt;
        //}

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            ShowMessagesAsync();
        }

        public string EncryptAndSignMessage(string message, string receiverPublicKey, string senderPrivateKey)
        {
            using (RSACryptoServiceProvider receiverRsa = new RSACryptoServiceProvider())
            {
                receiverRsa.FromXmlString(receiverPublicKey);

                // Mesajı alıcının public anahtarıyla şifrele
                byte[] encryptedBytes = receiverRsa.Encrypt(Encoding.UTF8.GetBytes(message), false);

                // Şifrelenmiş mesajı base64 formatında kodla
                string encryptedMessage = Convert.ToBase64String(encryptedBytes);

                return encryptedMessage;
            }
        }

        // Alıcının mesajı çözmesi
        //string decryptMessage = DecryptMessage(selectedEncryptedMessage, receiverPrivateKey, senderPublicKey, receiverPublicKey);
        private string DecryptMessage(string encryptedMessage, string receiverPrivateKey, string senderPublicKey, string receiverPublicKey)
        {
            using (RSACryptoServiceProvider receiverRsa = new RSACryptoServiceProvider())
            {
                receiverRsa.FromXmlString(receiverPrivateKey);

                // Şifrelenmiş mesajı çöz
                byte[] encryptedBytes = Convert.FromBase64String(encryptedMessage);
                byte[] decryptedBytes = receiverRsa.Decrypt(encryptedBytes, false); // Şifrelenmiş mesajı çözerken 'false' kullanıyoruz

                // Mesajı döndür
                string decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);

                //bool isVerifySignature = VerifySignature(encryptedMessage, receiverPrivateKey, senderPublicKey);

                return decryptedMessage;
            }
        }

        //İmzanın doğrulanması

        //public bool VerifySignature(string message, string receivedSenderPublicKey, string originalSenderPublicKey)
        //{
        //    using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
        //    using (RSACryptoServiceProvider senderRsa = new RSACryptoServiceProvider())
        //    {
        //        rsa.FromXmlString(receivedSenderPublicKey);
        //        senderRsa.FromXmlString(originalSenderPublicKey);

        //        // Veriyi SHA-256 ile imzala ve imzayı doğrula
        //        bool result = rsa.VerifyData(Encoding.UTF8.GetBytes(message), new SHA256CryptoServiceProvider(), senderRsa.ExportParameters(false).Modulus);

        //        return result;
        //    }
        //}

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
                string senderName = showUsername;
                string receiverName = TxtReceiver.Text;
                string message = richTextBox1.Text;

                // Gönderen ve alıcının keyleri alma
                string senderPrivateKey = GetPrivateKeyByUsername(senderName);
                string senderPublicKey = GetPublicKeyByUsername(senderName);
                string receiverPublicKey = GetPublicKeyByUsername(receiverName);
                string receiverPrivateKey = GetPrivateKeyByUsername(receiverName);

                // Mesajı şifrele ve imzala
                string signedMessage = EncryptAndSignMessage(message, receiverPublicKey, senderPrivateKey);

                // Gönderen ve alıcının ID'lerini al
                int senderID = GetUserIDByUsername(senderName);
                int receiverID = GetUserIDByUsername(receiverName);

                // Şifrelenmiş ve imzalanmış mesajı veritabanına kaydet
                SaveEncryptedMessageToDatabase(senderID, receiverID, signedMessage);

                MessageBox.Show("Mesaj başarıyla gönderildi.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ShowMessagesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
                int messageID = Convert.ToInt32(selectedRow.Cells["ID"].Value);
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
                    var keys = GetSenderAndReceiverKeysByMessageID(messageID);
                    var info = GetSenderNameAndDateByMessageID(messageID);
                    var title = GetTitleByMessageID(messageID);

                    if (!string.IsNullOrEmpty(encryptedMessage))
                    {
                        string encryptedMessageShow = ProcessKeys(encryptedMessage, keys.Item1, keys.Item2, keys.Item3);
                        MesajDetay msjDetay = new MesajDetay();
                        Mesaj msj = new Mesaj();
                        msjDetay.message = encryptedMessageShow;
                        msjDetay.senderName = info.Item1;
                        msjDetay.date = info.Item2;
                        msjDetay.messageID = messageID;
                        msjDetay.title = title;
                        msjDetay.MesajFormReference = this;         // Mesaj formunu referans olarak iletiyoruz
                        msjDetay.showUsername = showUsername;
                        msjDetay.Show();
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

        public string GetTitleByMessageID(int messageID)
        {
            string title = "";

            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT Title FROM TBLMESSAGES WHERE MessageID = @MessageID", connection))
                {
                    command.Parameters.AddWithValue("@MessageID", messageID);

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            title = dr["Title"].ToString();
                        }
                    }
                }

                connection.Close();
            }

            return title;
        }

        public Tuple<string, string, string> GetSenderAndReceiverKeysByMessageID(int messageID)
        {
            string senderPublicKey = "";
            string receiverPublicKey = "";
            string receiverPrivateKey = "";

            // Öncelikle, messageID ile ilişkili SenderID ve ReceiverID'yi alın
            int senderID = -1;
            int receiverID = -1;

            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT SenderID, ReceiverID FROM TBLMESSAGES WHERE MessageID = @MessageID", connection))
                {
                    command.Parameters.AddWithValue("@MessageID", messageID);

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            senderID = Convert.ToInt32(dr["SenderID"]);
                            receiverID = Convert.ToInt32(dr["ReceiverID"]);
                        }
                    }
                }

                connection.Close();
            }

            // Ardından, SenderID ile ilgili kullanıcının public anahtarını alın
            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT PublicKey FROM TBLUSERS WHERE UserID = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", senderID);

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            senderPublicKey = dr["PublicKey"].ToString();
                        }
                    }
                }

                connection.Close();
            }

            // Alıcının private ve public anahtarlarını almak için aynı işlemi tekrarlayın
            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT PublicKey, PrivateKey FROM TBLUSERS WHERE UserID = @UserID", connection))
                {
                    command.Parameters.AddWithValue("@UserID", receiverID);

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            receiverPublicKey = dr["PublicKey"].ToString();
                            receiverPrivateKey = dr["PrivateKey"].ToString();
                        }
                    }
                }

                connection.Close();
            }

            // Tuple kullanarak bu üç anahtarı döndürün
            return Tuple.Create(senderPublicKey, receiverPrivateKey, receiverPublicKey);
        }

        public string ProcessKeys(string selectedEncryptedMessage, string senderPublicKey, string receiverPrivateKey, string receiverPublicKey)
        {
            string decryptMessage = DecryptMessage(selectedEncryptedMessage, receiverPrivateKey, senderPublicKey, receiverPublicKey);
            return decryptMessage;
        }

        // MesajDetay formuna gönderilecek olan mesajı kimin gönderdiğini ve mesajın tarihini döndürür
        public Tuple<string, string> GetSenderNameAndDateByMessageID(int messageID)
        {
            string senderName = "";
            string date = "";

            using (SqlConnection connection = Connection.CreateConnection())
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand("SELECT SenderID, Timestamp FROM TBLMESSAGES WHERE MessageID = @MessageID", connection))
                {
                    command.Parameters.AddWithValue("@MessageID", messageID);

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            int senderID = Convert.ToInt32(dr["SenderID"]);
                            date = dr["Timestamp"].ToString();

                            using (SqlConnection connection2 = Connection.CreateConnection())
                            {
                                connection2.Open();

                                using (SqlCommand command2 = new SqlCommand("SELECT Username FROM TBLUSERS WHERE UserID = @UserID", connection2))
                                {
                                    command2.Parameters.AddWithValue("@UserID", senderID);

                                    using (SqlDataReader dr2 = command2.ExecuteReader())
                                    {
                                        if (dr2.Read())
                                        {
                                            senderName = dr2["Username"].ToString();
                                        }
                                    }
                                }

                                connection2.Close();
                            }
                        }
                    }
                }

                connection.Close();
            }

            return Tuple.Create(senderName, date);
        }

        private void Mesaj_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Kullanıcıya bir onay iletişimi gösterelim
            DialogResult result = MessageBox.Show("Formu kapatmak istiyor musunuz?", "Kapatma Onayı", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            // Kullanıcının seçimine göre işlem yapın
            if (result == DialogResult.Yes)
            {
                // Kullanıcı formun kapatılmasını istiyor
                StopFetchingData();
                Giris grs = new Giris();
                grs.Show();
                Mesaj msj = new Mesaj();
                msj.Close();
            }
            else
            {
                e.Cancel = true;
            }
        }



        private void BtnUsers_Click(object sender, EventArgs e)
        {
            Kullanicilar users = new Kullanicilar();
            users.Show();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            GidenMesaj gdnmsj = new GidenMesaj();
            gdnmsj.senderName = showUsername;
            gdnmsj.Show();
        }


    }
}

