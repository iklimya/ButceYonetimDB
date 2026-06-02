namespace _2.sınıf_2._dönem_projesi
{
    partial class Form2
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form2));
            panel1 = new Panel();
            mtbPhone = new MaskedTextBox();
            label4 = new Label();
            btnGeri = new Button();
            btnKayıt2 = new Button();
            txtSifre = new TextBox();
            txtKullaniciAdi = new TextBox();
            label2 = new Label();
            label1 = new Label();
            label3 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.Controls.Add(mtbPhone);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(btnGeri);
            panel1.Controls.Add(btnKayıt2);
            panel1.Controls.Add(txtSifre);
            panel1.Controls.Add(txtKullaniciAdi);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Location = new Point(548, 213);
            panel1.Name = "panel1";
            panel1.Size = new Size(593, 369);
            panel1.TabIndex = 2;
            // 
            // mtbPhone
            // 
            mtbPhone.Location = new Point(188, 170);
            mtbPhone.Mask = "+90 (000) 000-00-00";
            mtbPhone.Name = "mtbPhone";
            mtbPhone.Size = new Size(160, 27);
            mtbPhone.TabIndex = 4;
            mtbPhone.MaskInputRejected += mtbPhone_MaskInputRejected;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Tahoma", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label4.ForeColor = SystemColors.ButtonFace;
            label4.Location = new Point(3, 176);
            label4.Name = "label4";
            label4.Size = new Size(169, 21);
            label4.TabIndex = 6;
            label4.Text = "Telefon numarası :";
            //label4.Click += label4_Click;
            // 
            // btnGeri
            // 
            btnGeri.BackColor = Color.DarkKhaki;
            btnGeri.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnGeri.ForeColor = Color.WhiteSmoke;
            btnGeri.Location = new Point(325, 247);
            btnGeri.Name = "btnGeri";
            btnGeri.Size = new Size(167, 64);
            btnGeri.TabIndex = 5;
            btnGeri.Text = "geri";
            btnGeri.UseVisualStyleBackColor = false;
            btnGeri.Click += button2_Click;
            btnGeri.MouseEnter += button2_MouseEnter;
            btnGeri.MouseLeave += button2_MouseLeave;
            // 
            // btnKayıt2
            // 
            btnKayıt2.BackColor = Color.DarkGreen;
            btnKayıt2.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 162);
            btnKayıt2.ForeColor = Color.WhiteSmoke;
            btnKayıt2.Location = new Point(71, 247);
            btnKayıt2.Name = "btnKayıt2";
            btnKayıt2.Size = new Size(167, 64);
            btnKayıt2.TabIndex = 4;
            btnKayıt2.Text = "Kayıt ol";
            btnKayıt2.UseVisualStyleBackColor = false;
            btnKayıt2.Click += btnKayıt2_Click;
            btnKayıt2.MouseEnter += button1_MouseEnter;
            btnKayıt2.MouseLeave += button1_MouseLeave;
            // 
            // txtSifre
            // 
            txtSifre.Location = new Point(188, 125);
            txtSifre.Name = "txtSifre";
            txtSifre.Size = new Size(267, 27);
            txtSifre.TabIndex = 3;
           // txtSifre.TextChanged += txtSifre_TextChanged;
            // 
            // txtKullaniciAdi
            // 
            txtKullaniciAdi.Location = new Point(188, 73);
            txtKullaniciAdi.Name = "txtKullaniciAdi";
            txtKullaniciAdi.Size = new Size(267, 27);
            txtKullaniciAdi.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Tahoma", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label2.ForeColor = SystemColors.ButtonFace;
            label2.Location = new Point(113, 127);
            label2.Name = "label2";
            label2.Size = new Size(59, 21);
            label2.TabIndex = 1;
            label2.Text = "şifre :";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Tahoma", 10.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label1.ForeColor = SystemColors.ButtonFace;
            label1.Location = new Point(45, 75);
            label1.Name = "label1";
            label1.Size = new Size(127, 21);
            label1.TabIndex = 0;
            label1.Text = "Kullanıcı Adı :";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Transparent;
            label3.Font = new Font("Tahoma", 16.2F, FontStyle.Bold, GraphicsUnit.Point, 162);
            label3.ForeColor = SystemColors.ButtonFace;
            label3.Location = new Point(783, 167);
            label3.Name = "label3";
            label3.Size = new Size(236, 34);
            label3.TabIndex = 3;
            label3.Text = "Hemen kayıt ol!";
            // 
            // Form2
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Silver;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1319, 757);
            Controls.Add(label3);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form2";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form2";
            Load += Form2_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel panel1;
        private TextBox txtSifre;
        private TextBox txtKullaniciAdi;
        private Label label2;
        private Label label1;
        private Button btnKayıt;
        private Label label3;
        private Button btnGeri;
        private Button btnKayıt2;
        private Label label4;
        private MaskedTextBox mtbPhone;
    }
}