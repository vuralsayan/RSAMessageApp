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

        SqlConnection baglanti = new SqlConnection(@"Data Source=BTSTAJER08\MSSQLSERVER01;Initial Catalog=DbRsaMessage;Persist Security Info=True;User ID=vural; Password=vural123");
        public string showUsername;

        private void Mesaj_Load(object sender, EventArgs e)
        {
            LblUsername.Text = showUsername;
        }
    }
}
