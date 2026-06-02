using Microsoft.VisualBasic;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using Twilio;
using static _2.sınıf_2._dönem_projesi.Form3;

namespace _2.sınıf_2._dönem_projesi
{
    public partial class Form1 : Form
    {
        private int failedCount = 0;

        SqlConnection baglanti = new SqlConnection(
            "Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;");

        private string connString = @"Server=(localdb)\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;";
        private int currentUserId = 1;
        private int lockSeconds = 60;
        private string realPassword = "";

        Size originalSize;
        Point originalLocation;

        bool growing = false;
        int remainingTime = 60;

        Panel panelLock;
        Label lblCountdown;
        Label lblWarning;
        System.Windows.Forms.Timer lockTimer3;

        bool sifreGorunuyor = false;

        public Form1()
        {
            InitializeComponent();
        }

        void StartProfessionalLock()
        {
            remainingTime = 60;
            lblCountdown.Text = remainingTime.ToString();
            panelLock.Visible = true;
            panelLock.BringToFront();
            lockTimer3.Start();
        }

        private void MakeRoundedButton(Button btn, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, btn.Width, btn.Height);

            int diameter = radius * 2;

            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);

            path.CloseFigure();
            btn.Region = new Region(path);

            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            originalSize = btngiris.Size;
            originalLocation = btngiris.Location;

            MakeRoundedButton(btnKayıt1, 20);
            MakeRoundedButton(btngiris, 20);

            // Şifre başlangıçta gizli
            textsifre.PasswordChar = '*';
            textsifre.UseSystemPasswordChar = false;
           bool sifreGorunuyor = false;
            linkLabel1.Text = "Şifreyi Göster";

            // Lock timer oluştur
            lockTimer3 = new System.Windows.Forms.Timer();
            lockTimer3.Interval = 1000;
            lockTimer3.Tick += LockTimer_Tick;

            // 3 girişten sonra kilit ekranı oluşturma
            panelLock = new Panel();
            panelLock.Dock = DockStyle.Fill;
            panelLock.BackColor = Color.Transparent;
            panelLock.Visible = false;

            lblWarning = new Label();
            lblWarning.Text = "HESAP GEÇİCİ OLARAK KİLİTLENDİ";
            lblWarning.ForeColor = Color.OrangeRed;
            lblWarning.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblWarning.AutoSize = false;
            lblWarning.TextAlign = ContentAlignment.MiddleCenter;
            lblWarning.Dock = DockStyle.Top;
            lblWarning.Height = 100;

            lblCountdown = new Label();
            lblCountdown.Text = "60";
            lblCountdown.ForeColor = Color.White;
            lblCountdown.Font = new Font("Segoe UI", 48, FontStyle.Bold);
            lblCountdown.AutoSize = false;
            lblCountdown.TextAlign = ContentAlignment.MiddleCenter;
            lblCountdown.Dock = DockStyle.Fill;

            panelLock.Controls.Add(lblCountdown);
            panelLock.Controls.Add(lblWarning);

            this.Controls.Add(panelLock);
            panelLock.BringToFront();
        }

        private void LockTimer_Tick(object sender, EventArgs e)
        {
            remainingTime--;
            lblCountdown.Text = remainingTime.ToString();

            if (remainingTime <= 0)
            {
                lockTimer3.Stop();
                remainingTime = 60;
                panelLock.Visible = false;
                failedCount = 0;
            }
        }

        private void btngiris_MouseEnter(object sender, EventArgs e)
        {
            btngiris.BackColor = Color.Lime;
        }

        private void btngiris_MouseLeave(object sender, EventArgs e)
        {
            btngiris.BackColor = Color.MediumAquamarine;
        }

        private void btnkayıt_MouseEnter(object sender, EventArgs e)
        {
            btnKayıt1.BackColor = Color.Aqua;
        }

        private void btnkayıt_MouseLeave(object sender, EventArgs e)
        {
            btnKayıt1.BackColor = Color.SteelBlue;
        }

        private void btngiris_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textKullanıcıadı.Text) || string.IsNullOrWhiteSpace(textsifre.Text))
            {
                MessageBox.Show("Boş alan bırakmayınız!", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                baglanti.Open();

                SqlCommand komut = new SqlCommand(
                    "SELECT KullaniciID FROM Kullanicilar WHERE KullaniciAdi=@kadi AND Sifre=@sifre",
                    baglanti);

                komut.Parameters.AddWithValue("@kadi", textKullanıcıadı.Text);
                komut.Parameters.AddWithValue("@sifre", textsifre.Text);

                object sonuc = komut.ExecuteScalar();

                if (sonuc != null)
                {
                    failedCount = 0;
                    int gelenID = Convert.ToInt32(sonuc);
                    KullaniciBilgi.AktifKullaniciID = gelenID;
                    Form3 ana = new Form3(gelenID);
                    ana.Show();
                    this.Hide();
                }
                else
                {
                    failedCount++;

                    if (failedCount >= 3)
                    {
                        StartProfessionalLock();
                    }
                    else
                    {
                        MessageBox.Show($"Kullanıcı adı veya şifre hatalı ❌\nKalan hakkınız: {3 - failedCount}",
                            "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        textsifre.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            finally
            {
                baglanti.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
          
        }

        private void btnKayıt1_Click_1(object sender, EventArgs e)
        {
        }

        private void btnKayıt1_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
            this.Hide();
        }

        private void linkLabel1_LinkClicked_1(object sender, LinkLabelLinkClickedEventArgs e)
        {
            sifreGorunuyor = !sifreGorunuyor;

            if (sifreGorunuyor)
            {
                string yedek = textsifre.Text;

                textsifre.PasswordChar = (char)0;
                textsifre.Clear();
                textsifre.Text = yedek;
                linkLabel1.Text = "Şifreyi Gizle";
            }
            else
            {
                textsifre.PasswordChar = '*';

                linkLabel1.Text = "Şifreyi Göster";
            }
        }
    }
}