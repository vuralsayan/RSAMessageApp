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
        }

        public string message;
        public string senderName;
        public string date;

        private void MesajDetay_Load(object sender, EventArgs e)
        {
            LblMesaj.Text = message;
            LblSender.Text = senderName;
            LblDate.Text = date;
        }
    }
}
