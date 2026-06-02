namespace _2.sınıf_2._dönem_projesi
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            AnaBaslık = new Label();
            label1 = new Label();
            panel1 = new Panel();
            linkLabel1 = new LinkLabel();
            label3 = new Label();
            label2 = new Label();
            btnKayıt1 = new Button();
            btngiris = new Button();
            textsifre = new TextBox();
            textKullanıcıadı = new TextBox();
            lockTimer = new System.Windows.Forms.Timer(components);
            pictureBox1 = new PictureBox();
            toolTip1 = new ToolTip(components);
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // AnaBaslık
            // 
            AnaBaslık.AutoSize = true;
            AnaBaslık.BackColor = Color.Transparent;
            AnaBaslık.Font = new Font("Microsoft JhengHei UI", 22.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            AnaBaslık.ForeColor = Color.Black;
            AnaBaslık.ImageAlign = ContentAlignment.TopCenter;
            AnaBaslık.Location = new Point(715, 124);
            AnaBaslık.Name = "AnaBaslık";
            AnaBaslık.Size = new Size(402, 55);
            AnaBaslık.TabIndex = 0;
            AnaBaslık.Tag = "";
            AnaBaslık.Text = "Tekrar Hoş Geldiniz !";
            AnaBaslık.UseCompatibleTextRendering = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Location = new Point(947, 209);
            label1.Name = "label1";
            label1.Size = new Size(141, 20);
            label1.TabIndex = 1;
            label1.Text = "Lütfen bilginizi girin";
            // 
            // panel1
            // 
            panel1.BackColor = Color.Transparent;
            panel1.Controls.Add(linkLabel1);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(btnKayıt1);
            panel1.Controls.Add(btngiris);
            panel1.Controls.Add(textsifre);
            panel1.Controls.Add(textKullanıcıadı);
            panel1.Location = new Point(753, 249);
            panel1.Name = "panel1";
            panel1.Size = new Size(482, 406);
            panel1.TabIndex = 1;
            // 
            // linkLabel1
            // 
            linkLabel1.AutoSize = true;
            linkLabel1.LinkColor = Color.Black;
            linkLabel1.Location = new Point(351, 225);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(94, 20);
            linkLabel1.TabIndex = 8;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "şifreyi göster";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked_1;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label3.Location = new Point(172, 175);
            label3.Name = "label3";
            label3.Size = new Size(54, 25);
            label3.TabIndex = 7;
            label3.Text = "şifre :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 162);
            label2.Location = new Point(114, 80);
            label2.Name = "label2";
            label2.Size = new Size(112, 25);
            label2.TabIndex = 6;
            label2.Text = "kullanıcı adı :";
            // 
            // btnKayıt1
            // 
            btnKayıt1.BackColor = Color.SteelBlue;
            btnKayıt1.ForeColor = Color.White;
            btnKayıt1.Location = new Point(374, 269);
            btnKayıt1.Name = "btnKayıt1";
            btnKayıt1.Size = new Size(94, 60);
            btnKayıt1.TabIndex = 5;
            btnKayıt1.Text = " kayıt ol";
            toolTip1.SetToolTip(btnKayıt1, " henüz kayıt olmadın mı ?????😯");
            btnKayıt1.UseVisualStyleBackColor = false;
            btnKayıt1.Click += btnKayıt1_Click;
            // 
            // btngiris
            // 
            btngiris.BackColor = Color.MediumAquamarine;
            btngiris.ForeColor = Color.White;
            btngiris.Location = new Point(194, 269);
            btngiris.Name = "btngiris";
            btngiris.Size = new Size(94, 60);
            btngiris.TabIndex = 4;
            btngiris.Text = "giriş yap";
            btngiris.UseVisualStyleBackColor = false;
            btngiris.Click += btngiris_Click;
            // 
            // textsifre
            // 
            textsifre.BackColor = SystemColors.InactiveBorder;
            textsifre.Location = new Point(232, 162);
            textsifre.Multiline = true;
            textsifre.Name = "textsifre";
            textsifre.PasswordChar = '*';
            textsifre.Size = new Size(190, 54);
            textsifre.TabIndex = 3;
            textsifre.UseSystemPasswordChar = true;
            // 
            // textKullanıcıadı
            // 
            textKullanıcıadı.BackColor = SystemColors.InactiveBorder;
            textKullanıcıadı.Location = new Point(232, 67);
            textKullanıcıadı.Multiline = true;
            textKullanıcıadı.Name = "textKullanıcıadı";
            textKullanıcıadı.Size = new Size(190, 52);
            textKullanıcıadı.TabIndex = 2;
            // 
            // lockTimer
            // 
            lockTimer.Interval = 1000;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(101, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(426, 760);
            pictureBox1.TabIndex = 5;
            pictureBox1.TabStop = false;
            // 
            // toolTip1
            // 
            toolTip1.BackColor = Color.DarkGray;
            toolTip1.ForeColor = SystemColors.ControlText;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.DimGray;
            BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
            ClientSize = new Size(1316, 758);
            Controls.Add(pictureBox1);
            Controls.Add(panel1);
            Controls.Add(label1);
            Controls.Add(AnaBaslık);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label AnaBaslık;
        private Label label1;
        private Panel panel1;
        private TextBox textsifre;
        private TextBox textKullanıcıadı;
        private Button btnKayıt1;
        private Button btngiris;
        private System.Windows.Forms.Timer lockTimer;
        private PictureBox pictureBox1;
        private Label label2;
        private Label label3;
        private LinkLabel linkLabel1;
        private ToolTip toolTip1;
    }
}
