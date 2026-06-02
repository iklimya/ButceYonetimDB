using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Data.SqlClient;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Identity.Client;




namespace _2.sınıf_2._dönem_projesi
{
    public partial class Form2 : Form
    {


        SqlConnection baglanti = new SqlConnection(
    "Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;");


        public Form2()
        {
            InitializeComponent();
        }

        private void MakeRoundedButton(Button btn, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, btn.Width, btn.Height);

            int diameter = radius * 2;

            // Sol üst köşe
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            // Sağ üst köşe
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            // Sağ alt köşe
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            // Sol alt köşe
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);

            path.CloseFigure();
            btn.Region = new Region(path);

            // Kenarlığı kaldırmak için
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
        }

        private void Form2_Load(object sender, EventArgs e)
        {


            //panel2.SendToBack();
            btnKayıt2.FlatStyle = FlatStyle.Flat;
            btnKayıt2.FlatAppearance.BorderSize = 0;
            btnGeri.FlatStyle = FlatStyle.Flat;
            btnGeri.FlatAppearance.BorderSize = 0;


            MakeRoundedButton(btnKayıt2, 20); // 20 px radius
            MakeRoundedButton(btnGeri, 20);

        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            btnKayıt2.BackColor = Color.Lime;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            btnKayıt2.BackColor = Color.DarkGreen;
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            btnGeri.BackColor = Color.Gold;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            btnGeri.BackColor = Color.DarkKhaki;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Form1 form1 = new Form1();
            form1.Show();
            this.Close();
        }

        private void btnKayıt2_Click(object sender, EventArgs e)
        {
            //--------------------------------------------------------------
            string telefon = mtbPhone.Text;

            // Gereksiz karakterleri temizle ve uluslararası formata çevir
            telefon = telefon.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

            if (!telefon.StartsWith("+"))
            {
                telefon = "+90" + telefon.TrimStart('0');
            }

            //----------------------------------------------------------------

            if (txtKullaniciAdi.Text == "" || txtSifre.Text == "")
            {
                 MessageBox.Show("Boş alan bırakmayınız!");
               // OzelMesajFormu mesaj1 = new OzelMesajFormu("Boş alan bırakmayınız!", "hata", this);
              //  mesaj1.ShowDialog();
                return;
            }

            if (txtKullaniciAdi.Text == "" || txtSifre.Text == "")
            {
                 MessageBox.Show("Boş alan bırakmayınız!");
               // OzelMesajFormu mesaj2 = new OzelMesajFormu("Boş alan bırakmayınız!", "hata", this);
               // mesaj2.ShowDialog();
                return;
            }

            baglanti.Open();

            // Önce kullanıcı var mı kontrol ediyoruz
            SqlCommand kontrolKomut = new SqlCommand(
                "SELECT COUNT(*) FROM Kullanicilar WHERE KullaniciAdi=@kadi",
                baglanti);

            kontrolKomut.Parameters.AddWithValue("@kadi", txtKullaniciAdi.Text);

            int sonuc = (int)kontrolKomut.ExecuteScalar();

            if (sonuc > 0)
            {
                 MessageBox.Show("Bu kullanıcı adı zaten kayıtlı!");
               // OzelMesajFormu mesaj3 = new OzelMesajFormu("Bu kullanıcı zaten kayıtlı!", "hata", this);
              //  mesaj3.ShowDialog();

                txtKullaniciAdi.Clear();
                txtSifre.Clear();

                baglanti.Close();
                return;
            }

            // Eğer kullanıcı yoksa kayıt yap
            string iban = HesapNoUret();
            SqlCommand komut = new SqlCommand(
                "INSERT INTO Kullanicilar (KullaniciAdi, Sifre, TelefonNumarası,iban) VALUES (@kadi, @sifre,@telefon,@iban)",
                baglanti);

            komut.Parameters.AddWithValue("@kadi", txtKullaniciAdi.Text);
            komut.Parameters.AddWithValue("@sifre", txtSifre.Text);
            komut.Parameters.AddWithValue("@telefon", mtbPhone.Text);

            komut.Parameters.AddWithValue("@iban",iban);


            komut.ExecuteNonQuery();
            baglanti.Close();

             MessageBox.Show("Kayıt Yapıldı ✅");
            //OzelMesajFormu mesaj = new OzelMesajFormu("Kayıt Yapıldı ✅", "hata", this);
           // mesaj.ShowDialog();

            Form1 frm1 = new Form1();
            frm1.Show();
            this.Close();


           



        }

        private void mtbPhone_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            string phone = mtbPhone.Text.Trim(); // MaskedTextBox kullanıyoruz

            if (string.IsNullOrWhiteSpace(phone))
            {
                 MessageBox.Show("Lütfen telefon numarası girin!");
               // OzelMesajFormu mesaj = new OzelMesajFormu("Lütfen telefon numarası girin!", "hata", this);
               // mesaj.ShowDialog();
                return;
            }

            try
            {
                using (SqlConnection baglanti = new SqlConnection(
                    "Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
                {
                    baglanti.Open();

                    int currentUserId = 1; // Test için

                    SqlCommand cmd = new SqlCommand(
                        "UPDATE dbo.Users SET PhoneNumber=@phone WHERE UserId=@id", baglanti);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@id", currentUserId);

                    cmd.ExecuteNonQuery();
                }

                 MessageBox.Show("Telefon numaranız başarıyla kaydedildi!");
               // OzelMesajFormu mesaj = new OzelMesajFormu("Telefon numaranız başarıyla kaydedildi ✅", "hata", this);
               // mesaj.ShowDialog();
            }
            catch (Exception ex)
            {
                //MessageBox.Show("Hata: " + ex.Message);
            }
        }


        public string HesapNoUret()
        {
            Random rnd = new Random();
            string iban = "TR"; // 🔥 başına TR ekledik

            for (int i = 0; i < 16; i++) // 16 haneli sayı
            {
                iban += rnd.Next(0, 10).ToString();
            }

            return iban;
        }
    }

}
