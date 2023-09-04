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
            this.LblMesaj = new System.Windows.Forms.Label();
            this.LblSender = new System.Windows.Forms.Label();
            this.LblDate = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LblMesaj
            // 
            this.LblMesaj.AutoSize = true;
            this.LblMesaj.Location = new System.Drawing.Point(69, 45);
            this.LblMesaj.Name = "LblMesaj";
            this.LblMesaj.Size = new System.Drawing.Size(62, 24);
            this.LblMesaj.TabIndex = 0;
            this.LblMesaj.Text = "label1";
            // 
            // LblSender
            // 
            this.LblSender.AutoSize = true;
            this.LblSender.Location = new System.Drawing.Point(69, 106);
            this.LblSender.Name = "LblSender";
            this.LblSender.Size = new System.Drawing.Size(62, 24);
            this.LblSender.TabIndex = 1;
            this.LblSender.Text = "label1";
            // 
            // LblDate
            // 
            this.LblDate.AutoSize = true;
            this.LblDate.Location = new System.Drawing.Point(69, 159);
            this.LblDate.Name = "LblDate";
            this.LblDate.Size = new System.Drawing.Size(62, 24);
            this.LblDate.TabIndex = 2;
            this.LblDate.Text = "label1";
            // 
            // MesajDetay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(194)))), ((int)(((byte)(208)))));
            this.ClientSize = new System.Drawing.Size(914, 477);
            this.Controls.Add(this.LblDate);
            this.Controls.Add(this.LblSender);
            this.Controls.Add(this.LblMesaj);
            this.Font = new System.Drawing.Font("Georgia", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MesajDetay";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MesajDetay";
            this.Load += new System.EventHandler(this.MesajDetay_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblMesaj;
        private System.Windows.Forms.Label LblSender;
        private System.Windows.Forms.Label LblDate;
    }
}