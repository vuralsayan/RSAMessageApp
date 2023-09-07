namespace RSAMessageApp
{
    partial class MesajDetay
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MesajDetay));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.BtnDelete = new System.Windows.Forms.Button();
            this.BtnAnswer = new System.Windows.Forms.Button();
            this.TxtSender = new System.Windows.Forms.TextBox();
            this.TxtDate = new System.Windows.Forms.TextBox();
            this.TxtTitle = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(16, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(146, 29);
            this.label1.TabIndex = 3;
            this.label1.Text = "Gönderen:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(64, 135);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 29);
            this.label2.TabIndex = 4;
            this.label2.Text = "Tarih:";
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(449, 21);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(641, 205);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            // 
            // BtnDelete
            // 
            this.BtnDelete.Location = new System.Drawing.Point(21, 210);
            this.BtnDelete.Name = "BtnDelete";
            this.BtnDelete.Size = new System.Drawing.Size(194, 48);
            this.BtnDelete.TabIndex = 6;
            this.BtnDelete.Text = "Sil";
            this.BtnDelete.UseVisualStyleBackColor = true;
            this.BtnDelete.Click += new System.EventHandler(this.BtnDelete_Click);
            // 
            // BtnAnswer
            // 
            this.BtnAnswer.Location = new System.Drawing.Point(221, 210);
            this.BtnAnswer.Name = "BtnAnswer";
            this.BtnAnswer.Size = new System.Drawing.Size(194, 48);
            this.BtnAnswer.TabIndex = 7;
            this.BtnAnswer.Text = "Cevapla";
            this.BtnAnswer.UseVisualStyleBackColor = true;
            this.BtnAnswer.Click += new System.EventHandler(this.BtnAnswer_Click);
            // 
            // TxtSender
            // 
            this.TxtSender.Location = new System.Drawing.Point(158, 21);
            this.TxtSender.Name = "TxtSender";
            this.TxtSender.Size = new System.Drawing.Size(257, 35);
            this.TxtSender.TabIndex = 8;
            // 
            // TxtDate
            // 
            this.TxtDate.Location = new System.Drawing.Point(158, 129);
            this.TxtDate.Name = "TxtDate";
            this.TxtDate.Size = new System.Drawing.Size(257, 35);
            this.TxtDate.TabIndex = 9;
            // 
            // TxtTitle
            // 
            this.TxtTitle.Location = new System.Drawing.Point(158, 75);
            this.TxtTitle.Name = "TxtTitle";
            this.TxtTitle.Size = new System.Drawing.Size(257, 35);
            this.TxtTitle.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(64, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 29);
            this.label3.TabIndex = 10;
            this.label3.Text = "Başlık:";
            // 
            // MesajDetay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(14F, 29F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1102, 270);
            this.Controls.Add(this.TxtTitle);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.TxtDate);
            this.Controls.Add(this.TxtSender);
            this.Controls.Add(this.BtnAnswer);
            this.Controls.Add(this.BtnDelete);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MesajDetay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RSA Message App";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MesajDetay_FormClosing);
            this.Load += new System.EventHandler(this.MesajDetay_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Button BtnDelete;
        private System.Windows.Forms.Button BtnAnswer;
        private System.Windows.Forms.TextBox TxtSender;
        private System.Windows.Forms.TextBox TxtDate;
        private System.Windows.Forms.TextBox TxtTitle;
        private System.Windows.Forms.Label label3;
    }
}