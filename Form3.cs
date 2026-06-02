using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using static _2.sınıf_2._dönem_projesi.Form3;
using Newtonsoft.Json.Linq;
using Microsoft.Toolkit.Uwp.Notifications;


namespace _2.sınıf_2._dönem_projesi
{

    public partial class Form3 : Form
    {
        private int aktifKullaniciID;
        private Chart giderChart;
        private Chart giderChart1;
        private Chart haftalikGelirGrafik;
        int secilenArkadasID = -1;
        TextBox ekran;
        private bool bildirimAcik = true;
        private System.Windows.Forms.Timer bildirimTimer;
        private bool aiGiderYorumYazildi = false;
        private BildirimService _bildirimService;
        private int _sonKontrolID = 0;

        // 1️⃣ Timer tanımla
        // public int AktifKullaniciID1;
        string baglantiYolu = "Data Source=.;Initial Catalog=ButceYonetimDB;Integrated Security=True";

        public Form3(int KullaniciID)
        {
            InitializeComponent();

            aktifKullaniciID = KullaniciID;

            _bildirimService = new BildirimService(KullaniciID);
            BildirimTimerBaslat();


        }
        // private Timer grafikTimer;
        private Dictionary<string, decimal> hedefDegerler = new Dictionary<string, decimal>();
        private Dictionary<string, decimal> simdikiDegerler = new Dictionary<string, decimal>();

        private void ChartHazirla()
        {
            // Eğer daha önce eklenmişse temizle
            if (giderChart != null)
            {
                GiderAltPaneli1.Controls.Remove(giderChart);
                giderChart.Dispose();
            }

            // Yeni Chart oluştur
            giderChart = new Chart();
            giderChart.Dock = DockStyle.Fill; // paneli doldursun
            GiderAltPaneli1.Controls.Add(giderChart);

            ChartArea area = new ChartArea();
            giderChart.ChartAreas.Add(area);
        }


        SqlConnection baglanti = new SqlConnection(
 "Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;");
        public Form3()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

        }
        //formun rengi
        private bool aydinlikMod = false;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Color renk1, renk2;

            if (aydinlikMod)
            {
                renk1 = Color.FromArgb(65, 90, 119);
                renk2 = Color.FromArgb(119, 158, 196);
            }
            else
            {
                renk1 = Color.FromArgb(13, 27, 42);
                renk2 = Color.FromArgb(65, 90, 119);
            }

            LinearGradientBrush brush = new LinearGradientBrush(
                this.ClientRectangle,
                renk1,
                renk2,
                90F
            );

            e.Graphics.FillRectangle(brush, this.ClientRectangle);
        }
        private Label lblToplamBakiye;
        private TextBox txtTutar;
        private TextBox txtAciklama;
        private async void Form3_Load(object sender, EventArgs e)
        {



            MenuAltPaneli6Yukle();
            ProfilResminiGetir();
            // MenuAltPaneli5SaatlikHarcamaTrend();
            //MenuAltPaneli5SonIslemler();
            MenuAltPaneli2HareketGecmisiYukle();
            MenuAltPaneli4RozetYukle();
            MenuAltPaneli3TermometreYukle();

            button1.FlatAppearance.BorderColor = Color.Black;
            button2.FlatAppearance.BorderColor = Color.Lime;
            button3.FlatAppearance.BorderColor = Color.Black;
            button4.FlatAppearance.BorderColor = Color.Black;
            btntransfer.FlatAppearance.BorderColor = Color.Black;
            btnSohbet.FlatAppearance.BorderColor = Color.Black;


            //grafik yapma

            HaftalikGelirGrafikCiz();

            // Kullanıcının toplam bütçesini SQL’den al

            try
            {
                int aktifID = KullaniciBilgi.AktifKullaniciID;
                SqlConnection baglanti = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;");
                baglanti.Open();

                string sorgu = "SELECT SUM(Tutar) FROM Gelirler WHERE KullaniciID=@id";
                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@id", aktifID);

                object result = komut.ExecuteScalar();
                decimal toplamGelir = (result != DBNull.Value) ? Convert.ToDecimal(result) : 0;

                sorgu = "SELECT SUM(Tutar) FROM Harcamalar WHERE KullaniciID=@id";
                komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@id", aktifID);
                result = komut.ExecuteScalar();
                decimal toplamHarcama = (result != DBNull.Value) ? Convert.ToDecimal(result) : 0;

                lblToplamButce.Text = (toplamGelir - toplamHarcama).ToString("0.00");


                baglanti.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }



            int kullaniciID = KullaniciBilgi.AktifKullaniciID;
            //paneli oval yapma
            OvalPanelYap(ButcePaneli, 40);
            OvalPanelYap(GiderAltPaneli3, 40);
            OvalButonYap(button7, 30);
            OvalButonYap(btnParaGirisiniOnayla, 30);
            OvalButonYap(btnparagonder, 30);
            YuvarlakPanel(ProfilAltPaneli2, 40);
            YuvarlakPictureBox(pictureBoxProfil);
            try
            {
                string baglantiCumlesi = "Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;";
                using (SqlConnection baglanti = new SqlConnection(baglantiCumlesi))
                {
                    baglanti.Open();

                    SqlCommand komut = new SqlCommand(
                        "SELECT SUM(Tutar) FROM Gelirler WHERE KullaniciID=@kid",
                        baglanti);
                    komut.Parameters.AddWithValue("@kid", aktifKullaniciID);

                    object sonuc = komut.ExecuteScalar();
                    decimal toplamGelir = (sonuc != DBNull.Value) ? Convert.ToDecimal(sonuc) : 0;

                    lblToplamButce.Text = toplamGelir.ToString("N2");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }


            //ibanı sql den çekme ve txtiban adlı texboxa yazdırma
            string baglantikl = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(baglantikl))
            {
                try
                {
                    conn.Open();

                    string sql = "SELECT iban FROM kullanicilar WHERE KullaniciID = @id";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@id", aktifKullaniciID);

                    object sonuc = cmd.ExecuteScalar();

                    if (sonuc != null && sonuc != DBNull.Value)
                    {
                        txtiban.Text = sonuc.ToString();
                    }
                    else
                    {
                        txtiban.Text = "IBAN bulunamadı";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            //
            //kullanıcı adını sql den çekme ve txtiban adlı texboxa yazdırma
            string baglantiklm = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(baglantiklm))
            {
                try
                {
                    conn.Open();

                    string sql = "SELECT KullaniciAdi FROM kullanicilar WHERE KullaniciID = @ad";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@ad", aktifKullaniciID);

                    object sonuc = cmd.ExecuteScalar();

                    if (sonuc != null && sonuc != DBNull.Value)
                    {
                        textBox4.Text = sonuc.ToString();
                    }
                    else
                    {
                        txtiban.Text = "isim bulunamadı";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            //
            //sifre cekme
            string baglantiklma = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(baglantiklma))
            {
                try
                {
                    conn.Open();

                    string sql = "SELECT Sifre FROM kullanicilar WHERE KullaniciID = @sifre";

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.Parameters.AddWithValue("@sifre", aktifKullaniciID);

                    object sonuc = cmd.ExecuteScalar();

                    if (sonuc != null && sonuc != DBNull.Value)
                    {
                        textBox5.Text = sonuc.ToString(); // 🔒 otomatik ***** görünecek
                    }
                    else
                    {
                        textBox5.Text = "";
                        MessageBox.Show("Şifre bulunamadı");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            //


            ProfilPaneli.Visible = false;
            GelirPaneli.Visible = false;
            GiderPaneli.Visible = false;
            transferPaneli.Visible = false;
            MenuPaneli.Visible = true;
            pnlArkadasEkle.Visible = false;
            pnlSohbetEt.Visible = false;

            int radius = 200; // Köşe yuvarlaklık değeri (arttırırsan daha oval olur)


            SetPlaceholder(textBox3, "TR0011XXXXXXXXXXXX");
            SetPlaceholder(textBox6, "AXXXXX BXXXXX");
            SetPlaceholder(textBox7, "ALIŞVERİŞ");
            SetPlaceholder(textBox8, "XXXXX TL");
            SetPlaceholder(textBox2, "XXXXX TL");
            SetPlaceholder(textBox1, "XXXXX TL");



            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
            path.AddArc(new Rectangle(this.Width - radius, 0, radius, radius), 270, 90);
            path.AddArc(new Rectangle(this.Width - radius, this.Height - radius, radius, radius), 0, 90);
            path.AddArc(new Rectangle(0, this.Height - radius, radius, radius), 90, 90);
            path.CloseFigure();

            this.Region = new Region(path);

            //  MakeRoundedButton(button1, 70);
            MakeRoundedButton(button2, 70);
            MakeRoundedButton(button3, 70);
            MakeRoundedButton(button4, 70);
            MakeRoundedButton(btntransfer, 70);
            MakeRoundedButton(btntransfer, 70);
            MakeRoundedButton(btnResimSec, 40);
            MakeRoundedButton(button9, 70);
            MakeRoundedButton(button10, 70);
            MakeRoundedButton(button11, 70);
            // MakeRoundedButton(btnSifreDegis, 40);
            MakeRoundedButton(btnKaydet, 40);
            MakeRoundedButton(btnSohbet, 70);
            MakeRoundedButton(btnModDegistir, 40);
            MakeRoundedButton(btnKisiselBilgiler, 40);
            MakeRoundedButton(btnSifreVeGuvenlik, 40);
            //  MakeRoundedButton(btnZilSesiDegis, 40);
            MakeRoundedButton(btnpnlKisiselBilgilerDegistir, 40);
            MakeRoundedButton(btnPnlKisiselBilgilerGeri, 40);

            // MakeRoundedButton(btnBildirim_Click, 40);
            // MakeRoundedButton(button5, 70);


            button1.Width = 100;
            button1.Height = 100;

            GraphicsPath gp = new GraphicsPath();
            gp.AddEllipse(0, 0, button1.Width, button1.Height);
            button1.Region = new Region(gp);


            // Buton5 boyutunu eşit yap
            button5.Width = 50;
            button5.Height = 50;

            // Oval/dairesel yapmak için GraphicsPath
            GraphicsPath ab = new GraphicsPath();
            ab.AddEllipse(0, 0, button5.Width, button5.Height);
            button5.Region = new Region(ab);


            // Buton boyutunu eşit yap
            button1.Width = 100;
            button1.Height = 100;

            // Oval köşeler için GraphicsPath
            GraphicsPath ovalYolu = new GraphicsPath();

            int yariCap = 15; // köşe yarıçapı, burası artık radius değil, int olarak tanımlandı

            ovalYolu.StartFigure();
            ovalYolu.AddArc(new Rectangle(0, 0, yariCap, yariCap), 180, 90); // sol üst
            ovalYolu.AddArc(new Rectangle(kutuArama.Width - yariCap, 0, yariCap, yariCap), 270, 90); // sağ üst
            ovalYolu.AddArc(new Rectangle(kutuArama.Width - yariCap, kutuArama.Height - yariCap, yariCap, yariCap), 0, 90); // sağ alt
            ovalYolu.AddArc(new Rectangle(0, kutuArama.Height - yariCap, yariCap, yariCap), 90, 90); // sol alt
            ovalYolu.CloseFigure();

            kutuArama.Region = new Region(ovalYolu);


            //***********************
            try
            {
                using (SqlConnection baglanti = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
                {
                    baglanti.Open();

                    // Harcamalar toplamını al
                    string sorgu = "SELECT ISNULL(SUM(Tutar),0) FROM Harcamalar WHERE KullaniciID=@kid";
                    SqlCommand komut = new SqlCommand(sorgu, baglanti);
                    komut.Parameters.AddWithValue("@kid", aktifKullaniciID);

                    object sonuc = komut.ExecuteScalar();
                    decimal toplamHarcama = (sonuc != DBNull.Value) ? Convert.ToDecimal(sonuc) : 0;

                    // Label'da göster
                    lblHarcananButce.Text = "₺" + toplamHarcama.ToString("N2");

                    baglanti.Close();
                    KalanButceGuncelle();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
            HaftalikHarcamaGrafikCizHaftalik();
            KalanButceYuzdeGrafikCiz_Gelir();
            HaftalikGelirGrafikCiz();
            //MenuAltPaneli5SaatlikHarcamaTrend();
            PaneliOvalYap(transferPaneli, 100);
            PaneliOvalYap(pnlArkadasEkle, 100);
            PaneliOvalYap(pnlSohbetEt, 70);
            PaneliOvalYap(GiderAltPaneli4, 60);
            PaneliOvalYap(MenuAltPaneli1, 60);
            PaneliOvalYap(GiderAltPaneli5, 60);


            //sssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss

            dgvIstekler.CellContentClick += dgvIstekler_CellContentClick;

            dgvKullanicilar.Columns.Clear();

            dgvKullanicilar.Columns.Add("KullaniciID", "ID");
            dgvKullanicilar.Columns.Add("KullaniciAdi", "Kullanıcı Adı");
            dgvKullanicilar.Columns.Add("Durum", "Durum");

            dgvKullanicilar.Columns["KullaniciID"].Visible = false;



            dgvIstekler.Columns.Clear();

            dgvIstekler.Columns.Add("ID", "ID");
            dgvIstekler.Columns.Add("KullaniciAdi", "Kullanıcı");
            dgvIstekler.Columns.Add("Durum", "Durum");

            DataGridViewButtonColumn btnKabul = new DataGridViewButtonColumn();
            btnKabul.Name = "Kabul";
            btnKabul.Text = "Kabul Et";
            btnKabul.UseColumnTextForButtonValue = true;
            dgvIstekler.Columns.Add(btnKabul);

            DataGridViewButtonColumn btnRed = new DataGridViewButtonColumn();
            btnRed.Name = "Red";
            btnRed.Text = "Reddet";
            btnRed.UseColumnTextForButtonValue = true;
            dgvIstekler.Columns.Add(btnRed);

            dgvIstekler.Columns["ID"].Visible = false;

            KullanicilariGetir();
            IstekleriGetir();
            DataGridViewButtonColumn btn = new DataGridViewButtonColumn();
            btn.Name = "Ekle";
            btn.HeaderText = "İşlem";
            btn.Text = "Ekle";
            btn.UseColumnTextForButtonValue = true;

            dgvKullanicilar.Columns.Add(btn);

            dgvArkadaslar.Columns.Clear();

            dgvArkadaslar.Columns.Add("KullaniciID", "ID");
            dgvArkadaslar.Columns.Add("KullaniciAdi", "Arkadaş");

            // Buton 1
            DataGridViewButtonColumn btnChat = new DataGridViewButtonColumn();
            btnChat.Name = "Sohbet";
            btnChat.HeaderText = "";
            btnChat.Text = "Sohbet Et";
            btnChat.UseColumnTextForButtonValue = true;
            dgvArkadaslar.Columns.Add(btnChat);

            // Buton 2
            DataGridViewButtonColumn btnGroup = new DataGridViewButtonColumn();
            btnGroup.Name = "Grup";
            btnGroup.HeaderText = "";
            btnGroup.Text = "Grup Oluştur";
            btnGroup.UseColumnTextForButtonValue = true;
            dgvArkadaslar.Columns.Add(btnGroup);

            dgvArkadaslar.Columns["KullaniciID"].Visible = false;

            t.Interval = 1000;
            t.Tick += (s, e) => MesajlariGetir();
            t.Start();


            dgvArkadaslar.CellPainting += dgvArkadaslar_CellPainting;

            //ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss

            //*******************************************************************************************
            dgvArkadaslar.DefaultCellStyle.Padding = new Padding(10, 10, 10, 10);
            dgvArkadaslar.RowTemplate.Height = 70;

            pnlChat.AutoScroll = true;
            pnlChat.VerticalScroll.Value = pnlChat.VerticalScroll.Maximum;

            this.BackColor = Color.FromArgb(240, 242, 245); // soft gri
            pnlChat.BackColor = Color.Transparent;

            ArkadasGridiHazirla();
            GridiKartGibiYap(dgvKullanicilar);
            GridiKartGibiYap(dgvIstekler);
            GridiKartGibiYap(dgvArkadaslar);

            istekTimer.Interval = 1000; // 1 saniye
            istekTimer.Tick += IstekTimer_Tick;
            istekTimer.Start();

            IstekleriGetir();



            //******************************************************************************************
            HesapMakinesiniYukle(GiderAltPaneli4);

            //yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz
            AIYorumuBaslat();
            //yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz
            Label lblMod = new Label();
            lblMod.Name = "lblModYazisi";
            lblMod.ForeColor = Color.White;
            lblMod.BackColor = Color.Transparent;
            lblMod.Font = new Font("Courier New", 9, FontStyle.Regular);
            lblMod.AutoSize = true;
            lblMod.Text = "Karanlık Mod";
            lblMod.Location = new Point(btnModDegistir.Left + btnModDegistir.Width + 5, btnModDegistir.Top + 5);
            btnModDegistir.Parent.Controls.Add(lblMod);

            // Toggle butonu
            Button btnBildirim = new Button();
            btnBildirim.Size = new Size(114, 59);
            btnBildirim.Location = new Point(38, 280);
            btnBildirim.Text = "🔔";
            btnBildirim.Font = new Font("Arial", 14);
            btnBildirim.FlatStyle = FlatStyle.Flat;
            btnBildirim.BackColor = Color.Transparent;
            btnBildirim.ForeColor = Color.White;
            btnBildirim.FlatAppearance.BorderSize = 3;
            btnBildirim.FlatAppearance.BorderColor = Color.Lime;
            btnBildirim.Cursor = Cursors.Hand;
            ProfilAltPaneli2.Controls.Add(btnBildirim);
            MakeRoundedButton(btnBildirim, 40);


            // Yazı - butonun sağına ve ortasına hizalı
            Label lblBildirim = new Label();
            lblBildirim.Text = "Bildirimler";
            lblBildirim.ForeColor = Color.White;
            lblBildirim.Font = new Font("Courier New", 9, FontStyle.Regular);
            lblBildirim.Location = new Point(
                btnBildirim.Right + 10,                          // butonun sağından 10px boşluk
                btnBildirim.Top + (btnBildirim.Height / 2) - 8   // butona dikey ortalı
            );

            Label lblKisisel = new Label();
            lblKisisel.Text = "Kişisel Bilgileri Düzenle";
            lblKisisel.ForeColor = Color.White;
            lblKisisel.Font = new Font("Courier New", 9, FontStyle.Regular);
            lblKisisel.AutoSize = true;
            lblKisisel.Location = new Point(
                btnKisiselBilgiler.Location.X + btnKisiselBilgiler.Width + 5,
                btnKisiselBilgiler.Location.Y + (btnKisiselBilgiler.Height - lblKisisel.Height) / 2
            );

            ProfilAltPaneli2.Controls.Add(lblKisisel);

            Label lblSifreGuvenlik = new Label();
            lblSifreGuvenlik.Text = "Şifre Ve Güvenlik";
            lblSifreGuvenlik.ForeColor = Color.White;
            lblSifreGuvenlik.Font = new Font("Courier New", 9, FontStyle.Regular);
            lblSifreGuvenlik.AutoSize = true;
            lblSifreGuvenlik.Location = new Point(
                btnSifreVeGuvenlik.Location.X + btnSifreVeGuvenlik.Width + 5,
                btnSifreVeGuvenlik.Location.Y + (btnSifreVeGuvenlik.Height - lblSifreGuvenlik.Height) / 2
            );

            ProfilAltPaneli2.Controls.Add(lblSifreGuvenlik);

            Label lblZilSesi = new Label();
           // lblZilSesi.Text = "Zil Sesleri";
            lblZilSesi.ForeColor = Color.White;
            lblZilSesi.Font = new Font("Courier New", 9, FontStyle.Regular);
            lblZilSesi.AutoSize = true;
            lblZilSesi.Location = new Point(
            // btnZilSesiDegis.Location.X + btnZilSesiDegis.Width + 5,
            //  btnZilSesiDegis.Location.Y + (btnZilSesiDegis.Height - lblZilSesi.Height) / 2
            );

            ProfilAltPaneli2.Controls.Add(lblZilSesi);

            lblBildirim.AutoSize = true;
            ProfilAltPaneli2.Controls.Add(lblBildirim);
            // Tıklama olayı
            btnBildirim.Click += (s, e) =>
            {
                bildirimAcik = !bildirimAcik;

                if (bildirimAcik)
                {
                    btnBildirim.FlatAppearance.BorderColor = Color.Lime;
                    btnBildirim.Text = "🔔";
                }
                else
                {
                    btnBildirim.FlatAppearance.BorderColor = Color.Red;
                    btnBildirim.Text = "🔕";
                }
            };


            MenuAltPaneli1.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, MenuAltPaneli1.ClientRectangle,
                    Color.Lime, ButtonBorderStyle.Solid);
            };


            // KA TextBox için label
            Label lblKA = new Label();
            lblKA.Text = "Kullanıcı adı değişimi";
            lblKA.AutoSize = true;
            lblKA.ForeColor = Color.White; // Renge göre değiştir
            lblKA.DataBindings.Clear();

            // KA'nın konumuna göre label'ı yerleştir
            lblKA.Left = KA.Right + 5;
            lblKA.Top = KA.Top + (KA.Height - lblKA.Height) / 2;
            pnlKisiselBilgiler.Controls.Add(lblKA);

            // KA hareket edince label da hareket etsin
            KA.LocationChanged += (s, e) =>
            {
                lblKA.Left = KA.Right + 5;
                lblKA.Top = KA.Top + (KA.Height - lblKA.Height) / 2;
            };

            // TN TextBox için label
            Label lblTN = new Label();
            lblTN.Text = "Telefon numarası değişimi";
            lblTN.AutoSize = true;
            lblTN.ForeColor = Color.White;

            lblTN.Left = TN.Right + 5;
            lblTN.Top = TN.Top + (TN.Height - lblTN.Height) / 2;
            pnlKisiselBilgiler.Controls.Add(lblTN);




            // TN hareket edince label da hareket etsin
            TN.LocationChanged += (s, e) =>
            {
                lblTN.Left = TN.Right + 5;
                lblTN.Top = TN.Top + (TN.Height - lblTN.Height) / 2;
            };


            // TN TextBox için label
            Label lblsifrem = new Label();
            lblsifrem.Text = "Şifre değişimi";
            lblsifrem.AutoSize = true;
            lblsifrem.ForeColor = Color.White;

            lblsifrem.Left = textBox9.Right + 5;
            lblsifrem.Top = textBox9.Top + (TN.Height - lblsifrem.Height) / 2;
            pnlSifreVeGüvenlik.Controls.Add(lblsifrem);

        }

        //yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz
        private async Task AIYorumuBaslat()
        {
            try
            {
                // Veritabanından gelir ve harcama toplamını al
                decimal toplamGelir = 0, toplamHarcama = 0;

                using (SqlConnection baglanti = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
                {
                    baglanti.Open();

                    SqlCommand k1 = new SqlCommand("SELECT ISNULL(SUM(Tutar),0) FROM Gelirler WHERE KullaniciID=@id", baglanti);
                    k1.Parameters.AddWithValue("@id", aktifKullaniciID);
                    toplamGelir = Convert.ToDecimal(k1.ExecuteScalar());

                    SqlCommand k2 = new SqlCommand("SELECT ISNULL(SUM(Tutar),0) FROM Harcamalar WHERE KullaniciID=@id", baglanti);
                    k2.Parameters.AddWithValue("@id", aktifKullaniciID);
                    toplamHarcama = Convert.ToDecimal(k2.ExecuteScalar());
                }

                string yorum = await AIAsistan.HarcamaYorumuAl(toplamGelir, toplamHarcama);

                // Lime rengi label oluştur
                Label lblAI = new Label();
                lblAI.ForeColor = Color.Lime;
                lblAI.BackColor = Color.Transparent;
                lblAI.Font = new Font("Courier New", 11, FontStyle.Regular);
                lblAI.AutoSize = false;
                lblAI.Size = new Size(MenuAltPaneli1.Width - 20, MenuAltPaneli1.Height - 20);
                lblAI.Location = new Point(10, 10);
                lblAI.TextAlign = ContentAlignment.MiddleLeft;
                lblAI.Text = "";

                MenuAltPaneli1.Controls.Add(lblAI);

                // Yazma efekti - harf harf yaz
                await YazmaEfekti(lblAI, yorum);
            }
            catch (Exception ex)
            {
                MessageBox.Show("AI Hata: " + ex.Message);
            }
        }

        private async Task YazmaEfekti(Label lbl, string metin)
        {
            foreach (char harf in metin)
            {
                if (lbl.IsDisposed) break;
                lbl.Invoke((Action)(() => lbl.Text += harf));
                await Task.Delay(30); // hız - azaltırsan daha hızlı yazar
            }
        }

        //yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzzz

        private void MakeRoundedButton(Button btn, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();

            path.AddArc(new Rectangle(0, 0, radius, radius), 180, 90);
            path.AddArc(new Rectangle(btn.Width - radius, 0, radius, radius), 270, 90);
            path.AddArc(new Rectangle(btn.Width - radius, btn.Height - radius, radius, radius), 0, 90);
            path.AddArc(new Rectangle(0, btn.Height - radius, radius, radius), 90, 90);

            path.CloseFigure();
            btn.Region = new Region(path);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pnlSohbetEt.Visible = false;
            ProfilPaneli.Visible = false;
            GelirPaneli.Visible = false;
            GiderPaneli.Visible = false;
            transferPaneli.Visible = false;
            MenuPaneli.Visible = true;
            pnlArkadasEkle.Visible = false;
            // MenuAltPaneli5SaatlikHarcamaTrend();
            button1.FlatAppearance.BorderColor = Color.Black;
            button2.FlatAppearance.BorderColor = Color.Lime;
            button3.FlatAppearance.BorderColor = Color.Black;
            button4.FlatAppearance.BorderColor = Color.Black;
            btntransfer.FlatAppearance.BorderColor = Color.Black;
            btnSohbet.FlatAppearance.BorderColor = Color.Black;
            MenuAltPaneli2HareketGecmisiYukle();
        }

        private void button2_MouseEnter(object sender, EventArgs e)
        {
            button2.BackColor = Color.Lime;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.BackColor = Color.Transparent;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            pnlSohbetEt.Visible = false;
            GelirPaneli.Visible = false;
            GiderPaneli.Visible = false;
            MenuPaneli.Visible = false;
            transferPaneli.Visible = false;
            ProfilPaneli.Visible = true;
            pnlArkadasEkle.Visible = false;
            ProfilAltPaneli1.Visible = true;
            pnlKisiselBilgiler.Visible = false;
            pnlSifreVeGüvenlik.Visible = false;
            // pnlZilSesleri.Visible = false;

            button1.FlatAppearance.BorderColor = Color.Lime;
            button2.FlatAppearance.BorderColor = Color.Black;
            button3.FlatAppearance.BorderColor = Color.Black;
            button4.FlatAppearance.BorderColor = Color.Black;
            btntransfer.FlatAppearance.BorderColor = Color.Black;
            btnSohbet.FlatAppearance.BorderColor = Color.Black;


        }

        private void button1_MouseEnter(object sender, EventArgs e)
        {
            button1.BackColor = Color.Lime;
        }

        private void button1_MouseLeave(object sender, EventArgs e)
        {
            button1.BackColor = Color.Black;
        }

        private void button3_MouseEnter(object sender, EventArgs e)
        {
            button3.BackColor = Color.Lime;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            button3.BackColor = Color.Transparent;
        }

        private void button4_MouseEnter(object sender, EventArgs e)
        {
            button4.BackColor = Color.Lime;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            button4.BackColor = Color.Transparent;
        }

        private void button5_MouseEnter(object sender, EventArgs e)
        {
            button5.BackColor = Color.Lime;
        }

        private void button5_MouseLeave(object sender, EventArgs e)
        {
            button5.BackColor = Color.FromArgb(0, 180, 130);
        }


        private void button3_Click(object sender, EventArgs e)
        {
            GiderPaneli.Visible = false;
            MenuPaneli.Visible = false;
            ProfilPaneli.Visible = false;
            transferPaneli.Visible = false;
            pnlArkadasEkle.Visible = false;
            GelirPaneli.Visible = true;
            pnlSohbetEt.Visible = false;

            button1.FlatAppearance.BorderColor = Color.Black;
            button2.FlatAppearance.BorderColor = Color.Black;
            button3.FlatAppearance.BorderColor = Color.Lime;
            button4.FlatAppearance.BorderColor = Color.Black;
            btntransfer.FlatAppearance.BorderColor = Color.Black;
            btnSohbet.FlatAppearance.BorderColor = Color.Black;

        }

        private async void button4_Click(object sender, EventArgs e)
        {
            MenuPaneli.Visible = false;
            ProfilPaneli.Visible = false;
            GelirPaneli.Visible = false;
            pnlArkadasEkle.Visible = false;
            transferPaneli.Visible = false;
            GiderPaneli.Visible = true;
            pnlSohbetEt.Visible = false;
            GiderGrafikGuncelle();
            HaftalikHarcamaGrafikCizHaftalik();
            KalanButceYuzdeGrafikCiz_Gelir();
            HaftalikGelirGrafikCiz();
            // MenuAltPaneli5SaatlikHarcamaTrend();


            button1.FlatAppearance.BorderColor = Color.Black;
            button2.FlatAppearance.BorderColor = Color.Black;
            button3.FlatAppearance.BorderColor = Color.Black;
            button4.FlatAppearance.BorderColor = Color.Lime;
            btntransfer.FlatAppearance.BorderColor = Color.Black;
            btnSohbet.FlatAppearance.BorderColor = Color.Black;
            if (!aiGiderYorumYazildi)
            {
                aiGiderYorumYazildi = true;
                GiderAIYorumuBaslat();
            }
            GiderAltPaneli5.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, GiderAltPaneli5.ClientRectangle,
                    Color.Lime, ButtonBorderStyle.Solid);
            };
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Form1 frm1 = new Form1();
            frm1.Show();
            this.Close();
        }

        private async void btnParaGirisiniOnayla_Click_1(object sender, EventArgs e)
        {



        }
        private void OvalPanelYap(Panel panel, int yaricap)
        {
            if (panel.Width < yaricap * 2 || panel.Height < yaricap * 2)
                return; // panel çok küçükse hata verme

            GraphicsPath gp = new GraphicsPath();
            gp.StartFigure();

            // Sol üst köşe
            gp.AddArc(0, 0, yaricap, yaricap, 180, 90);
            // Sağ üst köşe
            gp.AddArc(panel.Width - yaricap, 0, yaricap, yaricap, 270, 90);
            // Sağ alt köşe
            gp.AddArc(panel.Width - yaricap, panel.Height - yaricap, yaricap, yaricap, 0, 90);
            // Sol alt köşe
            gp.AddArc(0, panel.Height - yaricap, yaricap, yaricap, 90, 90);

            gp.CloseFigure();

            panel.Region = new Region(gp);
        }
        private void OvalButonYap(Button btn, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btn.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btn.Width - radius, btn.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btn.Height - radius, radius, radius, 90, 90);
            path.CloseAllFigures();

            btn.Region = new Region(path);
        }


        private void GiderAltPaneli3_Paint(object sender, PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(
        GiderAltPaneli3.ClientRectangle,
        Color.FromArgb(173, 216, 230),   // koyu üst
        Color.FromArgb(135, 206, 250),   // biraz açılmış alt
        45F))
            {
                e.Graphics.FillRectangle(brush, GiderAltPaneli3.ClientRectangle);
            }

        }
        // Form1.cs veya ayrı bir static class içinde
        public static class KullaniciBilgi
        {
            public static int AktifKullaniciID = 0;
        }

        private void button7_Click(object sender, EventArgs e)
        {


            KalanButceGuncelle();
            GiderGrafikGuncelle();
            HaftalikHarcamaGrafikCizHaftalik();
            KalanButceYuzdeGrafikCiz_Gelir();
            HaftalikGelirGrafikCiz();
            // MenuAltPaneli5SaatlikHarcamaTrend();
            try
            {
                if (textBox2.Text == "" || comboBox1.SelectedIndex == -1)
                {
                    MessageBox.Show("Lütfen tutar ve kategori seçiniz.");
                    return;
                }

                decimal tutar = Convert.ToDecimal(textBox2.Text);
                string baslik = comboBox1.SelectedItem.ToString();

                using (SqlConnection baglanti = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
                {
                    baglanti.Open();

                    // 1️⃣ Harcamayı tabloya ekle
                    string query = "INSERT INTO Harcamalar (KullaniciID, Baslik, Tutar, Tarih) " +
                                   "VALUES (@kullaniciID, @baslik, @tutar, @tarih)";

                    using (SqlCommand cmd = new SqlCommand(query, baglanti))
                    {
                        cmd.Parameters.AddWithValue("@kullaniciID", aktifKullaniciID);
                        cmd.Parameters.AddWithValue("@baslik", baslik);
                        cmd.Parameters.AddWithValue("@tutar", tutar);
                        cmd.Parameters.AddWithValue("@tarih", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }

                    // 2️⃣ Güncel toplam harcamayı çek
                    string toplamQuery = "SELECT ISNULL(SUM(Tutar),0) FROM Harcamalar WHERE KullaniciID=@kid";
                    using (SqlCommand toplamCmd = new SqlCommand(toplamQuery, baglanti))
                    {
                        toplamCmd.Parameters.AddWithValue("@kid", aktifKullaniciID);
                        object sonuc = toplamCmd.ExecuteScalar();
                        decimal toplamHarcama = (sonuc != DBNull.Value) ? Convert.ToDecimal(sonuc) : 0;

                        // 3️⃣ Label’ı güncelle
                        lblHarcananButce.Text = "₺" + toplamHarcama.ToString("N2");

                        // Opsiyonel: Negatifse kırmızı, değilse yeşil
                        // lblHarcananButce.ForeColor = (toplamHarcama > 0) ? Color.Red : Color.Green;
                    }
                    KalanButceGuncelle();
                    HaftalikHarcamaGrafikCizHaftalik();
                    KalanButceYuzdeGrafikCiz_Gelir();
                    HaftalikGelirGrafikCiz();
                    // MenuAltPaneli5SaatlikHarcamaTrend();
                }

                // 4️⃣ Temizle
                textBox2.Clear();
                comboBox1.SelectedIndex = -1;

               // MessageBox.Show("Harcama kaydedildi ve toplam harcama güncellendi ✅");
            }
            catch (FormatException)
            {
             //   MessageBox.Show("Lütfen geçerli bir tutar girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            GiderGrafikGuncelle();
            HaftalikHarcamaGrafikCizHaftalik();
            KalanButceYuzdeGrafikCiz_Gelir();
            HaftalikGelirGrafikCiz();
            //MenuAltPaneli5SaatlikHarcamaTrend();


            decimal tutar1;
            if (!decimal.TryParse(textBox2.Text, out tutar1))
            {
               // MessageBox.Show("Lütfen geçerli bir tutar girin!");
                return;
            }

            try
            {
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Harcamalar (KullaniciID, Tutar, Tarih) VALUES (@id, @tutar, @tarih)",
                    baglanti);
                cmd.Parameters.AddWithValue("@id", aktifKullaniciID);
                cmd.Parameters.AddWithValue("@tutar", tutar1);
                cmd.Parameters.AddWithValue("@tarih", DateTime.Now);

                if (baglanti.State != System.Data.ConnectionState.Open)
                    baglanti.Open();

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Harcamayı eklerken hata: " + ex.Message);
                return;
            }
            finally
            {
                if (baglanti.State == System.Data.ConnectionState.Open)
                    baglanti.Close();
            }

            // Grafiği yeniden çiz
            // MenuAltPaneli5SaatlikHarcamaTrend();
        }
        private void KalanButceGuncelle()
        {
            try
            {
                decimal toplamButce = 0;
                decimal harcananButce = 0;

                if (!string.IsNullOrEmpty(lblToplamButce.Text))
                    decimal.TryParse(lblToplamButce.Text.Replace("₺", "").Trim(), out toplamButce);

                if (!string.IsNullOrEmpty(lblHarcananButce.Text))
                    decimal.TryParse(lblHarcananButce.Text.Replace("₺", "").Trim(), out harcananButce);

                decimal kalan = toplamButce - harcananButce;

                if (kalan < 0)
                {
                    kalan = 0;
                }

                lblKalanButce.Text = "₺" + kalan.ToString("N2");

                // 🔹 Kalan 0 ise button7 devre dışı ve uyarı
                if (kalan <= 0)
                {
                    button7.Enabled = false; // artık harcama eklenemez
                    lblKalanButce.ForeColor = Color.Red;
                }
                else
                {
                    button7.Enabled = true;
                    lblKalanButce.ForeColor = Color.Green;
                }
            }
            catch
            {
                lblKalanButce.Text = "₺0.00";
                button7.Enabled = false;
            }
        }
        private Panel legendPanel = null; // Bunu sınıfın en üstüne, diğer field'ların yanına ekle

        private void GiderGrafikGuncelle()
        {
            if (giderChart == null) ChartHazirla();

            giderChart.Series.Clear();
            giderChart.Titles.Clear();
            giderChart.Legends.Clear();

            // Başlık
            giderChart.Titles.Add("Gider Dağılımı");
            giderChart.Titles[0].ForeColor = Color.Cyan;
            giderChart.Titles[0].Font = new Font("Arial", 12, FontStyle.Bold);

            Series series = new Series
            {
                Name = "Giderler",
                IsValueShownAsLabel = true,
                ChartType = SeriesChartType.Doughnut,
                LabelForeColor = Color.Black,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            giderChart.Series.Add(series);

            Dictionary<string, Color> kategoriler = new Dictionary<string, Color>()
    {
        {"🍽 Yemek", Color.FromArgb(0, 255, 255)},
        {"🚌 Ulaşım", Color.FromArgb(0, 255, 128)},
        {"🏠 Fatura", Color.FromArgb(255, 0, 255)},
        {"🛍 Alışveriş", Color.FromArgb(255, 85, 0)},
        {"🏥 Sağlık", Color.FromArgb(0, 255, 0)},
        {"🎬 Eğlence", Color.FromArgb(255, 255, 0)},
        {"📚 Eğitim", Color.FromArgb(128, 0, 255)},
        {"📦 Diğer", Color.FromArgb(0, 128, 255)}
    };

            using (SqlConnection baglanti = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
            {
                baglanti.Open();

                foreach (var kat in kategoriler)
                {
                    SqlCommand cmd = new SqlCommand(
                        "SELECT SUM(Tutar) FROM Harcamalar WHERE KullaniciID=@kid AND Baslik=@baslik",
                        baglanti);
                    cmd.Parameters.AddWithValue("@kid", aktifKullaniciID);
                    cmd.Parameters.AddWithValue("@baslik", kat.Key);

                    object sonuc = cmd.ExecuteScalar();
                    decimal toplam = (sonuc != DBNull.Value) ? Convert.ToDecimal(sonuc) : 0;

                    if (toplam > 0)
                    {
                        int index = series.Points.AddXY(kat.Key, toplam);
                        series.Points[index].Color = kat.Value;
                        series.Points[index].BorderColor = Color.Black;
                        series.Points[index].BorderWidth = 2;
                        series["DoughnutRadius"] = "60";
                    }
                }
            }

            giderChart.BackColor = Color.Transparent;
            giderChart.ChartAreas[0].BackColor = Color.Transparent;

            // Panel ilk seferde oluştur, sonraki çağrılarda sadece içini temizle
            if (legendPanel == null)
            {
                legendPanel = new Panel();
                legendPanel.Dock = DockStyle.Right;
                legendPanel.Width = 150;
                legendPanel.BackColor = Color.Transparent;

                if (giderChart.Parent != null)
                {
                    giderChart.Parent.Controls.Add(legendPanel);
                    legendPanel.BringToFront();
                }
            }

            legendPanel.Controls.Clear();

            int y = 20;
            foreach (var kat in kategoriler)
            {
                Label lbl = new Label();
                lbl.Text = kat.Key;
                lbl.ForeColor = kat.Value;
                lbl.Font = new Font("Arial", 10, FontStyle.Bold);
                lbl.Location = new Point(10, y);
                lbl.AutoSize = true;
                legendPanel.Controls.Add(lbl);
                y += 25;
            }
        }



        //Grafik paneli 2-----------------------------------------------------------------------
        private Dictionary<DateTime, decimal> HaftalikHarcamaGetir()
        {
            Dictionary<DateTime, decimal> haftalikHarcama = new Dictionary<DateTime, decimal>();

            using (SqlConnection baglanti = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
            {
                baglanti.Open();
                string sorgu = @"
            SELECT CAST(Tarih AS DATE) as Gun, SUM(Tutar) as Toplam
            FROM Harcamalar
            WHERE KullaniciID=@kid AND Tarih >= DATEADD(day, -6, CAST(GETDATE() AS DATE))
            GROUP BY CAST(Tarih AS DATE)
            ORDER BY Gun";

                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                komut.Parameters.AddWithValue("@kid", aktifKullaniciID);

                SqlDataReader reader = komut.ExecuteReader();
                while (reader.Read())
                {
                    DateTime gun = Convert.ToDateTime(reader["Gun"]);
                    decimal toplam = reader["Toplam"] != DBNull.Value ? Convert.ToDecimal(reader["Toplam"]) : 0;
                    haftalikHarcama[gun] = toplam;
                }
                reader.Close();
            }

            return haftalikHarcama;
        }
        // -----------------------------------------
        private void HaftalikHarcamaGrafikCizHaftalik()
        {
            var haftalikHarcama = HaftalikHarcamaGetir();

            // Paneli temizle
            GiderAltPaneli2.Controls.Clear();

            // Chart oluştur
            Chart giderChartHaftalik = new Chart();
            giderChartHaftalik.Dock = DockStyle.Fill;
            giderChartHaftalik.BackColor = Color.Transparent;
            GiderAltPaneli2.Controls.Add(giderChartHaftalik);

            // ChartArea
            ChartArea areaHaftalik = new ChartArea("AreaHaftalik");
            areaHaftalik.BackColor = Color.Transparent;
            areaHaftalik.AxisX.LineColor = Color.Black;
            areaHaftalik.AxisY.LineColor = Color.Black;
            areaHaftalik.AxisX.LabelStyle.ForeColor = Color.FromArgb(150, 220, 255); // açık mavi
            areaHaftalik.AxisY.LabelStyle.ForeColor = Color.FromArgb(150, 220, 255); // açık mavi
            areaHaftalik.AxisX.LabelStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            areaHaftalik.AxisY.LabelStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            areaHaftalik.AxisX.MajorGrid.LineColor = Color.FromArgb(40, 40, 40); // hafif grid
            areaHaftalik.AxisY.MajorGrid.LineColor = Color.FromArgb(40, 40, 40);
            areaHaftalik.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            areaHaftalik.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            areaHaftalik.AxisX.LabelStyle.Angle = -45;
            giderChartHaftalik.ChartAreas.Add(areaHaftalik);

            // Series
            Series seriesHaftalik = new Series("HaftalikHarcama");
            seriesHaftalik.ChartType = SeriesChartType.Spline;
            seriesHaftalik.BorderWidth = 3;
            seriesHaftalik.Color = Color.FromArgb(0, 200, 255); // neon mavi çizgi
            seriesHaftalik.MarkerStyle = MarkerStyle.Circle;
            seriesHaftalik.MarkerSize = 8;
            seriesHaftalik.MarkerColor = Color.FromArgb(0, 200, 255); // nokta rengi çizgi ile uyumlu
            seriesHaftalik.IsValueShownAsLabel = true;
            seriesHaftalik.LabelForeColor = Color.FromArgb(150, 220, 255); // açık mavi
            seriesHaftalik.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            giderChartHaftalik.Series.Add(seriesHaftalik);

            // Haftanın her günü
            for (int i = 6; i >= 0; i--)
            {
                DateTime gun = DateTime.Today.AddDays(-i);
                decimal tutar = haftalikHarcama.ContainsKey(gun) ? haftalikHarcama[gun] : 0;
                seriesHaftalik.Points.AddXY(gun.ToString("ddd dd"), tutar);
            }

            // Legend kaldırıyoruz veya modern tasarım için eklemiyoruz
            giderChartHaftalik.Legends.Clear();
        }
        //-------------------------------------------
        private void KalanButceYuzdeGrafikCiz_Gelir()
        {
            GelirAltPaneli2.Controls.Clear();

            decimal toplam = 0;
            decimal kalan = 0;

            decimal.TryParse(lblToplamButce.Text.Replace("₺", ""), out toplam);
            decimal.TryParse(lblKalanButce.Text.Replace("₺", ""), out kalan);

            decimal harcanan = toplam - kalan;
            if (harcanan < 0) harcanan = 0;

            Chart yuzdeChart = new Chart();
            yuzdeChart.Dock = DockStyle.Fill;
            yuzdeChart.BackColor = Color.Transparent;
            GelirAltPaneli2.Controls.Add(yuzdeChart);

            ChartArea area = new ChartArea("AreaYuzde");
            area.BackColor = Color.Transparent;
            area.AxisX.Enabled = AxisEnabled.False;
            area.AxisY.Enabled = AxisEnabled.False;
            yuzdeChart.ChartAreas.Add(area);

            Series series = new Series("Bakiye");
            series.ChartType = SeriesChartType.Doughnut;
            series["DoughnutRadius"] = "50"; // Halka kalınlığı
            series["PieStartAngle"] = "270";

            series.Points.AddXY("Kalan", kalan);
            series.Points.AddXY("Harcanan", harcanan);

            series.Points[0].Color = Color.FromArgb(0, 200, 255); // Kalan mavi
            series.Points[1].Color = Color.FromArgb(200, 200, 200); // Harcanan gri
            series.IsValueShownAsLabel = false;

            yuzdeChart.Series.Add(series);
            yuzdeChart.Legends.Clear();

            // Çerçeve ve ortadaki yüzde
            yuzdeChart.Paint += (s, e) =>
            {
                Rectangle rect = yuzdeChart.ClientRectangle;
                int minSide = Math.Min(rect.Width, rect.Height);

                // Ortadaki yüzde
                using (Font font = new Font("Segoe UI", 28, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.White))
                {
                    string yuzde = toplam > 0 ? $"{(kalan / toplam * 100):0}%" : "0%";
                    SizeF size = e.Graphics.MeasureString(yuzde, font);
                    float x = (yuzdeChart.Width - size.Width) / 2;
                    float y = (yuzdeChart.Height - size.Height) / 2;
                    e.Graphics.DrawString(yuzde, font, brush, x, y);
                }
            };
        }


        private Dictionary<DateTime, decimal> HaftalikGelirGetir()
        {
            Dictionary<DateTime, decimal> gelirler = new Dictionary<DateTime, decimal>();

            using (SqlConnection baglanti = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
            {
                baglanti.Open();
                string sorgu = @"
            SELECT CONVERT(date,Tarih) AS Gun, SUM(Tutar) AS Toplam
            FROM Gelirler
            WHERE Tarih >= DATEADD(day,-6,GETDATE()) 
            GROUP BY CONVERT(date,Tarih)
            ORDER BY Gun";

                SqlCommand komut = new SqlCommand(sorgu, baglanti);
                SqlDataReader dr = komut.ExecuteReader();

                while (dr.Read())
                {
                    DateTime gun = Convert.ToDateTime(dr["Gun"]);
                    decimal toplam = Convert.ToDecimal(dr["Toplam"]);
                    gelirler[gun] = toplam;
                }
            }

            return gelirler;
        }

        private void HaftalikGelirGrafikCiz()
        {
            var haftalikGelir = HaftalikGelirGetir(); // DateTime -> decimal dönen Dictionary<DateTime, decimal>

            // Paneli temizle
            GelirAltPaneli3.Controls.Clear();

            // Chart oluştur
            Chart gelirChartHaftalik = new Chart();
            gelirChartHaftalik.Dock = DockStyle.Fill;
            gelirChartHaftalik.BackColor = Color.Transparent;
            GelirAltPaneli3.Controls.Add(gelirChartHaftalik);

            // ChartArea
            ChartArea areaHaftalik = new ChartArea("AreaHaftalikGelir");
            areaHaftalik.BackColor = Color.Transparent;
            areaHaftalik.AxisX.LineColor = Color.Black;
            areaHaftalik.AxisY.LineColor = Color.Black;

            areaHaftalik.AxisX.LabelStyle.ForeColor = Color.FromArgb(204, 0, 255);
            areaHaftalik.AxisY.LabelStyle.ForeColor = Color.FromArgb(204, 0, 255);
            areaHaftalik.AxisX.LabelStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            areaHaftalik.AxisY.LabelStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            areaHaftalik.AxisX.MajorGrid.LineColor = Color.FromArgb(40, 40, 40);
            areaHaftalik.AxisY.MajorGrid.LineColor = Color.FromArgb(40, 40, 40);
            areaHaftalik.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot;
            areaHaftalik.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot;

            areaHaftalik.AxisX.LabelStyle.Angle = -45;

            gelirChartHaftalik.ChartAreas.Add(areaHaftalik);

            // Series
            Series seriesHaftalik = new Series("HaftalikGelir");
            seriesHaftalik.ChartType = SeriesChartType.Spline; // akıcı çizgi
            seriesHaftalik.BorderWidth = 3;
            seriesHaftalik.Color = Color.FromArgb(204, 0, 255); // parlak mor
            seriesHaftalik.MarkerStyle = MarkerStyle.Circle;
            seriesHaftalik.MarkerSize = 8;
            seriesHaftalik.MarkerColor = Color.FromArgb(204, 0, 255);
            seriesHaftalik.IsValueShownAsLabel = true;
            seriesHaftalik.LabelForeColor = Color.FromArgb(204, 0, 255);
            seriesHaftalik.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            gelirChartHaftalik.Series.Add(seriesHaftalik);

            // Haftanın her günü (bugünden geriye 6 gün)
            for (int i = 6; i >= 0; i--)
            {
                DateTime gun = DateTime.Today.AddDays(-i);
                decimal tutar = haftalikGelir.ContainsKey(gun) ? haftalikGelir[gun] : 0;
                seriesHaftalik.Points.AddXY(gun.ToString("ddd dd"), tutar);
            }

            // Legend modern tasarım için kaldırıyoruz
            gelirChartHaftalik.Legends.Clear();
        }

        private void btnResimSec_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Profil Resmi Seç";
            ofd.Filter = "Resimler|*.jpg;*.jpeg;*.png;*.bmp;*.gif|Tüm Dosyalar|*.*";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Image secilenResim = Image.FromFile(ofd.FileName);

                // PictureBox ve button1'e uygula
                pictureBoxProfil.Image = secilenResim;
                pictureBoxProfil.SizeMode = PictureBoxSizeMode.StretchImage;

                button1.BackgroundImage = secilenResim;
                button1.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }
        private void YuvarlakPanel(Panel panel, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();

            // Panelin dört köşesini yuvarlak yap
            path.AddArc(0, 0, radius, radius, 180, 90); // Sol üst
            path.AddArc(panel.Width - radius, 0, radius, radius, 270, 90); // Sağ üst
            path.AddArc(panel.Width - radius, panel.Height - radius, radius, radius, 0, 90); // Sağ alt
            path.AddArc(0, panel.Height - radius, radius, radius, 90, 90); // Sol alt

            path.CloseFigure();
            panel.Region = new Region(path);
        }

        private void YuvarlakPictureBox(PictureBox pb)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, pb.Width, pb.Height); // Ellipse = Daire
            pb.Region = new Region(path);
        }
        private void btnKaydet_Click(object sender, EventArgs e)
        {
            button1.Text = " ";



            if (pictureBoxProfil.Image != null)
            {
                try
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        pictureBoxProfil.Image.Save(ms, pictureBoxProfil.Image.RawFormat);
                        byte[] resim = ms.ToArray();

                        using (SqlConnection baglanti = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
                        {
                            SqlCommand komut = new SqlCommand("UPDATE Kullanicilar SET ProfilResim=@resim WHERE KullaniciID=@id", baglanti);
                            komut.Parameters.AddWithValue("@resim", resim);
                            komut.Parameters.AddWithValue("@id", aktifKullaniciID);

                            baglanti.Open();
                            komut.ExecuteNonQuery();
                            baglanti.Close();
                        }
                    }

                    MessageBox.Show("Profil resmi kaydedildi!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Önce bir resim seçin.");
            }
        }
        private void ProfilResminiGetir()
        {
            try
            {
                using (SqlConnection baglanti = new SqlConnection("Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
                {
                    SqlCommand komut = new SqlCommand("SELECT ProfilResim FROM Kullanicilar WHERE KullaniciID=@id", baglanti);
                    komut.Parameters.AddWithValue("@id", aktifKullaniciID);

                    baglanti.Open();
                    object sonuc = komut.ExecuteScalar();

                    if (sonuc != null && sonuc != DBNull.Value)
                    {
                        byte[] resim = (byte[])sonuc;
                        using (MemoryStream ms = new MemoryStream(resim))
                        {
                            Image img = Image.FromStream(ms);

                            pictureBoxProfil.Image = img;
                            pictureBoxProfil.SizeMode = PictureBoxSizeMode.StretchImage;

                            button1.BackgroundImage = img;
                            button1.BackgroundImageLayout = ImageLayout.Stretch;
                        }
                    }

                    baglanti.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message);
            }
        }
        /*
                // Menü paneli için saatlik harcama trendi grafiği
                private void MenuAltPaneli5SaatlikHarcamaTrend()
                {
                    MenuAltPaneli5.Controls.Clear();

                    Chart chart = new Chart();
                    chart.Dock = DockStyle.Fill;
                    chart.BackColor = System.Drawing.Color.Transparent;

                    ChartArea area = new ChartArea("AnaAlan");
                    area.BackColor = System.Drawing.Color.Transparent;
                    area.AxisX.LineWidth = 0;
                    area.AxisY.LineWidth = 0;
                    area.AxisX.MajorGrid.Enabled = false;
                    area.AxisY.MajorGrid.Enabled = false;
                    area.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
                    area.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
                    area.AxisX.Title = "Saat";
                    area.AxisY.Title = "Harcanan Tutar";
                    chart.ChartAreas.Add(area);

                    Series harcamaSeri = new Series("Harcamalar");
                    harcamaSeri.ChartType = SeriesChartType.Line;
                    harcamaSeri.Color = System.Drawing.Color.FromArgb(255, 7, 58);
                    harcamaSeri.BorderWidth = 3;
                    chart.Series.Add(harcamaSeri);

                    try
                    {
                        if (baglanti.State != System.Data.ConnectionState.Open)
                            baglanti.Open();

                        DateTime bugun = DateTime.Today;

                        for (int saat = 0; saat < 24; saat++)
                        {
                            SqlCommand cmdHarcama = new SqlCommand(
                                "SELECT SUM(Tutar) FROM Harcamalar WHERE KullaniciID=@id AND CAST(Tarih AS DATE)=@tarih AND DATEPART(HOUR, Tarih)=@saat",
                                baglanti);
                            cmdHarcama.Parameters.AddWithValue("@id", aktifKullaniciID);
                            cmdHarcama.Parameters.AddWithValue("@tarih", bugun);
                            cmdHarcama.Parameters.AddWithValue("@saat", saat);

                            object harcamaObj = cmdHarcama.ExecuteScalar();
                            decimal harcama = harcamaObj != DBNull.Value ? Convert.ToDecimal(harcamaObj) : 0;

                            harcamaSeri.Points.AddXY(saat.ToString("D2"), harcama);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Saatlik harcama grafiği hatası: " + ex.Message);
                    }
                    finally
                    {
                        if (baglanti.State == System.Data.ConnectionState.Open)
                            baglanti.Close();
                    }

                    MenuAltPaneli4.Controls.Add(chart);
                }
                */
        private void MenuAltPaneli5SonIslemler()
        {
            // Paneli temizle

            // DataGridView oluştur
            DataGridView dgvSonIslemler = new DataGridView();
            dgvSonIslemler.Dock = DockStyle.Fill;
            dgvSonIslemler.BackgroundColor = Color.FromArgb(30, 30, 30);
            dgvSonIslemler.BorderStyle = BorderStyle.None;
            dgvSonIslemler.EnableHeadersVisualStyles = false;
            dgvSonIslemler.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(45, 45, 48);
            dgvSonIslemler.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSonIslemler.RowHeadersVisible = false;
            dgvSonIslemler.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSonIslemler.ReadOnly = true;
            dgvSonIslemler.AllowUserToAddRows = false;
            dgvSonIslemler.AllowUserToDeleteRows = false;
            dgvSonIslemler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Kolonlar
            dgvSonIslemler.Columns.Add("Tur", "Tür");
            dgvSonIslemler.Columns.Add("Tutar", "Tutar");
            dgvSonIslemler.Columns.Add("TarihSaat", "Tarih Saat");

            try
            {
                if (baglanti.State != System.Data.ConnectionState.Open)
                    baglanti.Open();

                SqlCommand cmd = new SqlCommand(
                    @"SELECT 'Gelir' AS Tur, Tutar, Tarih FROM Gelirler WHERE KullaniciID=@id
              UNION ALL
              SELECT 'Gider' AS Tur, Tutar, Tarih FROM Harcamalar WHERE KullaniciID=@id
              ORDER BY Tarih DESC
              OFFSET 0 ROWS FETCH NEXT 20 ROWS ONLY",
                    baglanti);
                cmd.Parameters.AddWithValue("@id", aktifKullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    int rowIndex = dgvSonIslemler.Rows.Add();
                    dgvSonIslemler.Rows[rowIndex].Cells["Tur"].Value = dr["Tur"];
                    dgvSonIslemler.Rows[rowIndex].Cells["Tutar"].Value = dr["Tutar"];
                    dgvSonIslemler.Rows[rowIndex].Cells["TarihSaat"].Value = ((DateTime)dr["Tarih"]).ToString("dd.MM.yyyy HH:mm");

                    // Satır rengi
                    if (dr["Tur"].ToString() == "Gider")
                    {
                        dgvSonIslemler.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;
                        dgvSonIslemler.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
                    }
                    else
                    {
                        dgvSonIslemler.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Cyan;
                        dgvSonIslemler.Rows[rowIndex].DefaultCellStyle.BackColor = Color.FromArgb(255, 255, 255);
                    }
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Son işlemler yüklenirken hata: " + ex.Message);
            }
            finally
            {
                if (baglanti.State == System.Data.ConnectionState.Open)
                    baglanti.Close();
            }

            // DataGridView’i panelin içine ekle
            MenuAltPaneli5.Controls.Add(dgvSonIslemler);
            dgvSonIslemler.BringToFront(); // Görünürlüğü garantiye al
        }

        private void btntransfer_MouseEnter(object sender, EventArgs e)
        {
            btntransfer.BackColor = Color.Lime;
        }

        private void btntransfer_MouseLeave(object sender, EventArgs e)
        {
            btntransfer.BackColor = Color.Transparent;
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            transferPaneli.BackColor = Color.FromArgb(15, 32, 80);
        }

        private void btntransfer_Click(object sender, EventArgs e)
        {
            ProfilPaneli.Visible = false;
            GelirPaneli.Visible = false;
            GiderPaneli.Visible = false;
            transferPaneli.Visible = true;
            MenuPaneli.Visible = false;
            pnlArkadasEkle.Visible = false;
            pnlSohbetEt.Visible = false;

            button1.FlatAppearance.BorderColor = Color.Black;
            button2.FlatAppearance.BorderColor = Color.Black;
            button3.FlatAppearance.BorderColor = Color.Black;
            button4.FlatAppearance.BorderColor = Color.Black;
            btntransfer.FlatAppearance.BorderColor = Color.Lime;
            btnSohbet.FlatAppearance.BorderColor = Color.Black;
        }

        private void btnparagonder_Click(object sender, EventArgs e)
        {

            string baglantiYolu = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(baglantiYolu))
            {
                conn.Open();
                SqlTransaction trans = conn.BeginTransaction();

                try
                {
                    string aliciIban = textBox3.Text.Trim();
                    string aciklama = textBox7.Text.Trim();

                    if (!decimal.TryParse(textBox8.Text, out decimal tutar) || tutar <= 0)
                        throw new Exception("Geçerli tutar giriniz!");

                    if (string.IsNullOrWhiteSpace(aliciIban))
                        throw new Exception("IBAN boş olamaz!");

                    if (string.IsNullOrWhiteSpace(aciklama))
                        throw new Exception("Açıklama boş olamaz!");

                    // 1. ALICI BUL
                    SqlCommand aliciBul = new SqlCommand(
                        "SELECT KullaniciID FROM Kullanicilar WHERE Iban=@iban",
                        conn, trans);

                    aliciBul.Parameters.AddWithValue("@iban", aliciIban);

                    object sonuc = aliciBul.ExecuteScalar();

                    if (sonuc == null)
                        throw new Exception("IBAN bulunamadı!");

                    int aliciID = Convert.ToInt32(sonuc);

                    if (aliciID == aktifKullaniciID) throw new Exception("kendi hesabınıza para gönderemezsiniz");
                        // 2. GELİR (ALICIYA)
                        SqlCommand gelir = new SqlCommand(
                        "INSERT INTO Gelirler (KullaniciID, Tutar, Tarih, Aciklama) VALUES (@k, @t, @tr, @a)",
                        conn, trans);

                    gelir.Parameters.AddWithValue("@k", aliciID);
                    gelir.Parameters.AddWithValue("@t", tutar);
                    gelir.Parameters.AddWithValue("@tr", DateTime.Now);
                    gelir.Parameters.AddWithValue("@a", "Para Geldi: " + aciklama);

                    gelir.ExecuteNonQuery();

                    // 3. HARCAMA (GÖNDEREN)
                    SqlCommand harcama = new SqlCommand(
                        "INSERT INTO Harcamalar (KullaniciID, Tutar, Tarih, Baslik) VALUES (@k, @t, @tr, @b)",
                        conn, trans);

                    harcama.Parameters.AddWithValue("@k", aktifKullaniciID);
                    harcama.Parameters.AddWithValue("@t", tutar);
                    harcama.Parameters.AddWithValue("@tr", DateTime.Now);
                    harcama.Parameters.AddWithValue("@b", "Para Gönderildi: " + aciklama);

                    harcama.ExecuteNonQuery();

                    // 🔔 4. ALICI BİLDİRİMİ (BİLDİRİMLERİM)
                    SqlCommand bildirimAlici = new SqlCommand(
                        "INSERT INTO dbo.Bildirimlerim (KullaniciID, Mesaj, Tarih, Okundu) VALUES (@k, @m, @t, 0)",
                        conn, trans);

                    bildirimAlici.Parameters.AddWithValue("@k", aliciID);
                    bildirimAlici.Parameters.AddWithValue("@m", "Hesabınıza " + tutar + " TL gönderildi. Açıklama: " + aciklama);
                    bildirimAlici.Parameters.AddWithValue("@t", DateTime.Now);

                    bildirimAlici.ExecuteNonQuery();

                    // 🔔 5. GÖNDEREN BİLDİRİMİ
                    SqlCommand bildirimGonderen = new SqlCommand(
                        "INSERT INTO dbo.Bildirimlerim (KullaniciID, Mesaj, Tarih, Okundu) VALUES (@k, @m, @t, 0)",
                        conn, trans);

                    bildirimGonderen.Parameters.AddWithValue("@k", aktifKullaniciID);
                    bildirimGonderen.Parameters.AddWithValue(
                        "@m",
                       "Hesabınızdan " + tutar + " TL gönderildi. Açıklama: " + aciklama
                             );
                    bildirimGonderen.Parameters.AddWithValue("@t", DateTime.Now);

                    bildirimGonderen.ExecuteNonQuery();



                    // 6. ONAY
                    trans.Commit();

                    MessageBox.Show("Para transferi başarılı!");

                    textBox3.Text = "";
                    textBox8.Text = "";
                    textBox7.Text = "";
                    textBox6.Text = "";

                    KalanButceyiGuncelle();
                }
                catch (Exception ex)
                {
                    try { trans.Rollback(); } catch { }

                    MessageBox.Show("HATA: " + ex.Message);
                }
            }


        }

        private void KalanButceyiGuncelle()
        {
            string baglantiYolu = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(baglantiYolu))
            {
                conn.Open();

                // Gelir toplamı
                SqlCommand gelirCmd = new SqlCommand(
                    "SELECT ISNULL(SUM(Tutar),0) FROM Gelirler WHERE KullaniciID=@id",
                    conn);

                gelirCmd.Parameters.AddWithValue("@id", aktifKullaniciID);
                decimal gelir = (decimal)gelirCmd.ExecuteScalar();

                // Gider toplamı
                SqlCommand giderCmd = new SqlCommand(
                    "SELECT ISNULL(SUM(Tutar),0) FROM Harcamalar WHERE KullaniciID=@id",
                    conn);

                giderCmd.Parameters.AddWithValue("@id", aktifKullaniciID);
                decimal gider = (decimal)giderCmd.ExecuteScalar();

                decimal kalan = gelir - gider;

                lblKalanButce.Text = kalan.ToString("0.00") + " ₺";
            }
        }

        void MenuAltPaneli6Yukle()
        {
            string baglantiYolu = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(baglantiYolu))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "SELECT BildirimID, Mesaj FROM dbo.Bildirimlerim WHERE KullaniciID=@id ORDER BY BildirimID DESC",
                    conn);

                cmd.Parameters.AddWithValue("@id", aktifKullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();

                MenuAltPaneli6.Controls.Clear();

                // 🔥 SAĞ ALT KONUM
                MenuAltPaneli6.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                MenuAltPaneli6.Location = new Point(
                    this.ClientSize.Width - MenuAltPaneli6.Width - 10,
                    this.ClientSize.Height - MenuAltPaneli6.Height - 10
                );

                while (dr.Read())
                {
                    int id = Convert.ToInt32(dr["BildirimID"]);
                    string mesaj = dr["Mesaj"].ToString();

                    Panel pnl = new Panel();
                    pnl.Width = 300;
                    pnl.Height = 80;
                    pnl.BackColor = Color.FromArgb(70, 0, 0, 0); // şeffaf siyah
                    pnl.Margin = new Padding(5);

                    Label lbl = new Label();
                    lbl.Text = mesaj;
                    lbl.ForeColor = Color.White;
                    lbl.Width = 280;
                    lbl.Height = 40;
                    lbl.Location = new Point(5, 10);

                    Button btn = new Button();
                    btn.Text = "Tamam";
                    btn.Width = 70;
                    btn.Height = 25;
                    btn.Location = new Point(200, 50);

                    btn.Click += (s, e) =>
                    {
                        BildirimiSil(id);
                        MenuAltPaneli6Yukle(); // yenile
                    };

                    pnl.Controls.Add(lbl);
                    pnl.Controls.Add(btn);

                    MenuAltPaneli6.Controls.Add(pnl);
                }

                dr.Close();
            }
        }
        void BildirimiSil(int id)
        {
            string baglantiYolu = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True";

            using (SqlConnection conn = new SqlConnection(baglantiYolu))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM dbo.Bildirimlerim WHERE BildirimID=@id",
                    conn);

                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }
        }


        void PaneliOvalYap(Panel pnl, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            path.StartFigure();

            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(pnl.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(pnl.Width - radius, pnl.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, pnl.Height - radius, radius, radius, 90, 90);

            path.CloseFigure();

            pnl.Region = new Region(path);
        }



        void SetPlaceholder(TextBox txt, string placeholder)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            txt.Enter += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                }
            };

            txt.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                }
            };
        }

        private void btnSohbet_MouseEnter(object sender, EventArgs e)
        {
            btnSohbet.BackColor = Color.Lime;
        }

        private void btnSohbet_MouseLeave(object sender, EventArgs e)
        {
            btnSohbet.BackColor = Color.Transparent;
        }

        private void btnSohbet_Click(object sender, EventArgs e)
        {
            ProfilPaneli.Visible = false;
            GelirPaneli.Visible = false;
            GiderPaneli.Visible = false;
            transferPaneli.Visible = false;
            MenuPaneli.Visible = false;
            pnlArkadasEkle.Visible = true;
            pnlSohbetEt.Visible = true;



            dgvIstekler.Visible = false;
            dgvKullanicilar.Visible = false;

            dgvArkadaslar.Visible = true;
            pnlSohbetEt.Visible = true;

            button1.FlatAppearance.BorderColor = Color.Black;
            button2.FlatAppearance.BorderColor = Color.Black;
            button3.FlatAppearance.BorderColor = Color.Black;
            button4.FlatAppearance.BorderColor = Color.Black;
            btntransfer.FlatAppearance.BorderColor = Color.Black;
            btnSohbet.FlatAppearance.BorderColor = Color.Lime;

            IstekleriGetir();
            ArkadaslariGetir();
        }

        private void sohbetPaneli_Paint(object sender, PaintEventArgs e)
        {
            pnlArkadasEkle.BackColor = Color.FromArgb(10, 20, 50);
        }


        void MesajEkle(string mesaj, bool benimMi)
        {
            Label lbl = new Label();
            lbl.Text = mesaj;
            lbl.AutoSize = true;
            lbl.MaximumSize = new Size(250, 0);
            lbl.Padding = new Padding(10);
            lbl.Font = new Font("Segoe UI", 10);

            if (benimMi)
            {
                lbl.BackColor = Color.LightGreen;
                lbl.Left = pnlArkadasEkle.Width - lbl.Width - 25;
            }
            else
            {
                lbl.BackColor = Color.White;
                lbl.Left = 10;
            }

            lbl.Top = pnlArkadasEkle.Controls.Count * 50;

            pnlArkadasEkle.Controls.Add(lbl);
        }

        //ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss

        void KullanicilariGetir()
        {
            dgvKullanicilar.Rows.Clear();

            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True"))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
        SELECT k.KullaniciID, k.KullaniciAdi,
        (
            SELECT TOP 1 Durum 
            FROM Arkadasliklar 
            WHERE 
            (GonderenID = @ben AND AliciID = k.KullaniciID)
            OR 
            (GonderenID = k.KullaniciID AND AliciID = @ben)
        ) AS Durum
        FROM Kullanicilar k
        WHERE k.KullaniciID != @ben", conn);

                cmd.Parameters.AddWithValue("@ben", aktifKullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    string durum = dr["Durum"] == DBNull.Value ? "Ekle" : dr["Durum"].ToString();

                    dgvKullanicilar.Rows.Add(
                        dr["KullaniciID"],
                        dr["KullaniciAdi"],
                        durum
                    );
                }
            }
        }
        private void dgvKullanicilar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int secilenID = Convert.ToInt32(dgvKullanicilar.Rows[e.RowIndex].Cells["KullaniciID"].Value);

            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True"))
            {
                conn.Open();

                SqlCommand kontrol = new SqlCommand(@"
        SELECT COUNT(*) FROM Arkadasliklar 
        WHERE 
        (GonderenID=@ben AND AliciID=@sec)
        OR
        (GonderenID=@sec AND AliciID=@ben)", conn);

                kontrol.Parameters.AddWithValue("@ben", aktifKullaniciID);
                kontrol.Parameters.AddWithValue("@sec", secilenID);

                int varMi = (int)kontrol.ExecuteScalar();

                if (varMi == 0)
                {
                    SqlCommand ekle = new SqlCommand(@"
            INSERT INTO Arkadasliklar (GonderenID, AliciID, Durum)
            VALUES (@ben, @sec, 'Bekliyor')", conn);

                    ekle.Parameters.AddWithValue("@ben", aktifKullaniciID);
                    ekle.Parameters.AddWithValue("@sec", secilenID);

                    ekle.ExecuteNonQuery();

                    MessageBox.Show("İstek gönderildi 👍");
                    IstekleriGetir();
                    ArkadaslariGetir();
                }
                else
                {
                    MessageBox.Show("Zaten istek var veya arkadaşsınız");
                }
            }

            KullanicilariGetir();
        }
        void IstekleriGetir()
        {
            dgvIstekler.Rows.Clear();

            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True"))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
        SELECT a.ID, k.KullaniciAdi
        FROM Arkadasliklar a
        INNER JOIN Kullanicilar k ON a.GonderenID = k.KullaniciID
        WHERE a.AliciID = @ben AND a.Durum = 'Bekliyor'", conn);

                cmd.Parameters.AddWithValue("@ben", aktifKullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    dgvIstekler.Rows.Add(
                        dr["ID"],
                        dr["KullaniciAdi"]
                    );
                }
            }

            // 👉 Eğer hiç istek yoksa paneli gizle (opsiyonel)
            // pnlIstekler.Visible = dgvIstekler.Rows.Count > 0;
        }

        private void dgvKullanicilar_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dgvKullanicilar.Columns[e.ColumnIndex].Name != "Ekle")
                return;

            int secilenID = Convert.ToInt32(dgvKullanicilar.Rows[e.RowIndex].Cells["KullaniciID"].Value);

            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True"))
            {
                conn.Open();

                SqlCommand kontrol = new SqlCommand(@"
        SELECT COUNT(*) FROM Arkadasliklar
        WHERE (GonderenID=@ben AND AliciID=@sec)
        OR (GonderenID=@sec AND AliciID=@ben)", conn);

                kontrol.Parameters.AddWithValue("@ben", aktifKullaniciID);
                kontrol.Parameters.AddWithValue("@sec", secilenID);

                int varMi = (int)kontrol.ExecuteScalar();

                if (varMi == 0)
                {
                    SqlCommand ekle = new SqlCommand(@"
            INSERT INTO Arkadasliklar (GonderenID, AliciID, Durum)
            VALUES (@ben, @sec, 'Bekliyor')", conn);

                    ekle.Parameters.AddWithValue("@ben", aktifKullaniciID);
                    ekle.Parameters.AddWithValue("@sec", secilenID);

                    ekle.ExecuteNonQuery();




                    MessageBox.Show("İstek gönderildi 👍");
                }
                else
                {
                    MessageBox.Show("Zaten istek var");
                }
            }

            KullanicilariGetir();
            IstekleriGetir();
            ArkadaslariGetir();
        }
        void ArkadaslariGetir()
        {
            dgvArkadaslar.Rows.Clear();

            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True"))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
        SELECT k.KullaniciID, k.KullaniciAdi
        FROM Arkadasliklar a
        INNER JOIN Kullanicilar k 
        ON (
            (a.GonderenID = k.KullaniciID AND a.AliciID = @ben)
            OR
            (a.AliciID = k.KullaniciID AND a.GonderenID = @ben)
        )
        WHERE a.Durum = 'Kabul'
        AND k.KullaniciID != @ben", conn);

                cmd.Parameters.AddWithValue("@ben", aktifKullaniciID);

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    dgvArkadaslar.Rows.Add(
                        dr["KullaniciID"],
                        dr["KullaniciAdi"]
                    );
                }
            }
        }

        private void dgvArkadaslar_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            secilenArkadasID = Convert.ToInt32(
                dgvArkadaslar.Rows[e.RowIndex].Cells["KullaniciID"].Value
            );

            pnlArkadasEkle.Visible = true;
            pnlSohbetEt.Visible = true;

            MesajlariGetir();
        }

        private void dgvIstekler_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            istekTimer.Stop();

            int istekID = Convert.ToInt32(
                dgvIstekler.Rows[e.RowIndex].Cells[0].Value
            );

            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True"))
            {
                conn.Open();

                if (dgvIstekler.Columns[e.ColumnIndex].Name == "Kabul")
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Arkadasliklar SET Durum='Kabul' WHERE ID=@id", conn);
                    cmd.Parameters.AddWithValue("@id", istekID);
                    cmd.ExecuteNonQuery();
                }
                else if (dgvIstekler.Columns[e.ColumnIndex].Name == "Red")
                {
                    SqlCommand cmd = new SqlCommand("UPDATE Arkadasliklar SET Durum='Reddedildi' WHERE ID=@id", conn);
                    cmd.Parameters.AddWithValue("@id", istekID);
                    cmd.ExecuteNonQuery();
                }
            }

            IstekleriGetir();
            ArkadaslariGetir();

            istekTimer.Start();
        }
        private void btnGonder_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMesaj.Text)) return;

            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True"))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
        INSERT INTO Mesajlar (GonderenID, AliciID, Mesaj, Tarih)
        VALUES (@g, @a, @m, GETDATE())", conn);

                cmd.Parameters.AddWithValue("@g", aktifKullaniciID);
                cmd.Parameters.AddWithValue("@a", secilenArkadasID);
                cmd.Parameters.AddWithValue("@m", txtMesaj.Text);

                cmd.ExecuteNonQuery();
            }

            txtMesaj.Clear();
            MesajlariGetir();
        }


        void MesajlariGetir()
        {
            pnlChat.Controls.Clear();

            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True"))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
        SELECT * FROM Mesajlar
        WHERE 
        (GonderenID=@ben AND AliciID=@sec)
        OR 
        (GonderenID=@sec AND AliciID=@ben)
        ORDER BY Tarih", conn);

                cmd.Parameters.AddWithValue("@ben", aktifKullaniciID);
                cmd.Parameters.AddWithValue("@sec", secilenArkadasID);

                SqlDataReader dr = cmd.ExecuteReader();

                int y = 10; // 🔥 dikey sıra

                while (dr.Read())
                {
                    string mesaj = dr["Mesaj"].ToString();
                    int gonderen = Convert.ToInt32(dr["GonderenID"]);

                    Label lbl = new Label();
                    lbl.Text = mesaj;
                    lbl.AutoSize = true;
                    lbl.MaximumSize = new Size(200, 0);
                    lbl.Padding = new Padding(10);

                    Panel bubble = new Panel();
                    bubble.AutoSize = true;
                    bubble.Padding = new Padding(5);

                    // 🟢 SENİN MESAJIN (SAĞ)
                    if (gonderen == aktifKullaniciID)
                    {
                        lbl.BackColor = Color.LightGreen;

                        bubble.BackColor = Color.Transparent;
                        bubble.Refresh();
                        int margin = 10;

                        bubble.Left = pnlChat.ClientSize.Width - bubble.Width - margin;

                    }
                    else
                    {
                        // 🔵 KARŞI MESAJ (SOL)
                        lbl.BackColor = Color.LightGray;

                        bubble.Location = new Point(
                            10, // sola yasla
                            y
                        );
                    }

                    bubble.Controls.Add(lbl);
                    pnlChat.Controls.Add(bubble);

                    y += 60; // mesajlar aşağı insin
                }
           if(pnlChat.Controls.Count > 0)
                    pnlChat.ScrollControlIntoView(pnlChat.Controls[pnlChat.Controls.Count - 1]);
            
            }
        }
        //ssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssssss

        //**************************************************************
        void GridiKartGibiYap(DataGridView dgv)
        {
            dgv.BorderStyle = BorderStyle.None;
            dgv.BackgroundColor = Color.FromArgb(35, 55, 80);

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersVisible = false;

            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AllowUserToResizeColumns = false;

            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;

            dgv.DefaultCellStyle.BackColor = Color.FromArgb(35, 55, 80);
            dgv.DefaultCellStyle.ForeColor = Color.Black;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(240, 248, 255); // hafif mavi
            dgv.DefaultCellStyle.SelectionForeColor = Color.Black;

            dgv.GridColor = Color.White; // çizgileri yok et

            dgv.RowTemplate.Height = 60; // kart yüksekliği
        }


        void ArkadasGridiHazirla()
        {
            dgvArkadaslar.Columns.Clear();

            dgvArkadaslar.Columns.Add("KullaniciID", "ID");
            dgvArkadaslar.Columns.Add("KullaniciAdi", "Ad");

            // Sohbet butonu
            DataGridViewButtonColumn btnChat = new DataGridViewButtonColumn();
            btnChat.Name = "Sohbet";
            btnChat.Text = "Sohbet Et";
            btnChat.UseColumnTextForButtonValue = true;
            dgvArkadaslar.Columns.Add(btnChat);

            dgvArkadaslar.Columns["KullaniciID"].Visible = false;

            // stil
            dgvArkadaslar.Columns["KullaniciAdi"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            dgvArkadaslar.Columns["KullaniciAdi"].DefaultCellStyle.Font =
                new Font("Segoe UI", 11, FontStyle.Bold);

            dgvArkadaslar.Columns["Sohbet"].Width = 110;

            dgvArkadaslar.Columns["Sohbet"].DefaultCellStyle.BackColor = Color.FromArgb(255, 0, 0);
            dgvArkadaslar.Columns["Sohbet"].DefaultCellStyle.ForeColor = Color.FromArgb(255, 0, 0);
            dgvArkadaslar.Columns["Sohbet"].DefaultCellStyle.Font =
                new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void dgvArkadaslar_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                dgvArkadaslar.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
        }

        private void dgvArkadaslar_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                dgvArkadaslar.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(35, 55, 80);
        }

        int eskiIstekSayisi = 0;
        int sonIstekSayisi = 0;
        private void IstekTimer_Tick(object sender, EventArgs e)
        {
            int yeniSayi = IstekSayisiniGetir();

            if (yeniSayi > eskiIstekSayisi)
            {
                MessageBox.Show("Yeni arkadaşlık isteği var 🔔");
            }

            eskiIstekSayisi = yeniSayi;

            IstekleriGetir();
        }

        int IstekSayisiniGetir()
        {
            using (SqlConnection conn = new SqlConnection("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ButceYonetimDB;Integrated Security=True"))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
        SELECT COUNT(*) FROM Arkadasliklar
        WHERE AliciID = @ben AND Durum = 'Bekliyor'", conn);

                cmd.Parameters.AddWithValue("@ben", aktifKullaniciID);

                return (int)cmd.ExecuteScalar();
            }
        }

        private void istekTimer_Tick_1(object sender, EventArgs e)
        {
            int yeniSayi = IstekSayisiniGetir();

            // 🔥 SADECE YENİ GELİRSE GÖSTER
            if (yeniSayi > sonIstekSayisi)
            {
                MenuAltPaneli6.Visible = true;

            }

            sonIstekSayisi = yeniSayi;

            IstekleriGetir();
        }
        private void dgvArkadaslar_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (dgvArkadaslar.Columns[e.ColumnIndex].Name == "Sohbet" && e.RowIndex >= 0)
            {
                e.PaintBackground(e.CellBounds, true);

                using (Pen pen = new Pen(Color.Red, 2))
                {
                    Rectangle r = e.CellBounds;
                    r.Width -= 1;
                    r.Height -= 1;
                    e.Graphics.DrawRectangle(pen, r);
                }

                e.PaintContent(e.CellBounds);
                e.Handled = true;
            }
        }

        //**************************************************************

        private void HesapMakinesiniYukle(Panel panel)
        {
            panel.Controls.Clear();
            panel.BackColor = Color.FromArgb(25, 25, 25);

            // ================= EKRAN =================
            ekran = new TextBox();
            ekran.Width = panel.Width - 20;
            ekran.Height = 70;
            ekran.Left = 10;
            ekran.Top = 10;

            ekran.Font = new Font("Segoe UI", 20, FontStyle.Bold);
            ekran.TextAlign = HorizontalAlignment.Right;
            ekran.BackColor = Color.FromArgb(40, 40, 40);
            ekran.ForeColor = Color.Lime;
            ekran.BorderStyle = BorderStyle.None;

            panel.Controls.Add(ekran);

            // ================= GRID HESAP =================
            int satir = 5;
            int sutun = 4;

            int bosluk = 8;

            int usableHeight = panel.Height - ekran.Bottom - 10;

            int btnW = (panel.Width - (bosluk * (sutun + 1))) / sutun;
            int btnH = (usableHeight - (bosluk * (satir + 1))) / satir;

            string[,] btns =
            {
        {"7","8","9","/"},
        {"4","5","6","*"},
        {"1","2","3","-"},
        {"0","C","=","+"},
        {".","←","%","√"}
    };

            for (int r = 0; r < satir; r++)
            {
                for (int c = 0; c < sutun; c++)
                {
                    RoundedButton b = new RoundedButton();

                    b.Text = btns[r, c];

                    b.Width = btnW;
                    b.Height = btnH;

                    b.Left = bosluk + c * (btnW + bosluk);
                    b.Top = ekran.Bottom + bosluk + r * (btnH + bosluk);

                    b.Font = new Font("Segoe UI", 12, FontStyle.Bold);

                    b.BackColor = Color.FromArgb(60 + r * 8, 90 + c * 8, 150);
                    b.ForeColor = Color.White;

                    b.FlatStyle = FlatStyle.Flat;
                    b.FlatAppearance.BorderSize = 0;

                    b.Click += Btn_Click;

                    panel.Controls.Add(b);
                }
            }
        }
        public class RoundedButton : Button
        {
            protected override void OnResize(EventArgs e)
            {
                base.OnResize(e);

                int radius = 12;

                GraphicsPath path = new GraphicsPath();
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(Width - radius, Height - radius, radius, radius, 0, 90);
                path.AddArc(0, Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();

                this.Region = new Region(path);
            }
        }

        private void Btn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            if (btn == null) return;

            string val = btn.Text;

            try
            {
                // TEMİZLE
                if (val == "C")
                {
                    ekran.Text = "";
                    return;
                }

                // SİL
                if (val == "←")
                {
                    if (ekran.Text.Length > 0)
                        ekran.Text = ekran.Text.Substring(0, ekran.Text.Length - 1);
                    return;
                }

                // EŞİTTİR
                if (val == "=")
                {
                    if (string.IsNullOrWhiteSpace(ekran.Text)) return;

                    var sonuc = new System.Data.DataTable().Compute(ekran.Text, null);
                    ekran.Text = sonuc.ToString();
                    return;
                }

                // KAREKÖK
                if (val == "√")
                {
                    if (string.IsNullOrEmpty(ekran.Text)) return;

                    double sayi = Convert.ToDouble(ekran.Text);
                    ekran.Text = Math.Sqrt(sayi).ToString();
                    return;
                }

                // YÜZDE
                if (val == "%")
                {
                    if (string.IsNullOrEmpty(ekran.Text)) return;

                    double sayi = Convert.ToDouble(ekran.Text);
                    ekran.Text = (sayi / 100).ToString();
                    return;
                }

                // OPERATÖR KONTROLÜ
                string ops = "+-*/";

                if (ops.Contains(val))
                {
                    if (string.IsNullOrEmpty(ekran.Text)) return;

                    char son = ekran.Text[ekran.Text.Length - 1];

                    if (ops.Contains(son))
                        ekran.Text = ekran.Text.Substring(0, ekran.Text.Length - 1) + val;
                    else
                        ekran.Text += val;

                    return;
                }

                // NORMAL SAYI
                ekran.Text += val;

            }
            catch
            {
                ekran.Text = "Hata";
            }
        }

        private void btnModDegistir_Click(object sender, EventArgs e)
        {
            aydinlikMod = !aydinlikMod;

            if (aydinlikMod)
            {
                btnModDegistir.Text = "☀️";
                btnModDegistir.ForeColor = Color.Yellow;
            }
            else
            {
                btnModDegistir.Text = "🌙";
                btnModDegistir.ForeColor = Color.White;
            }

            this.Invalidate();

            // Butonu bulduk, label'ı yanına ekleyelim
            Label lblMod = new Label();
            lblMod.Name = "lblModYazisi";
            lblMod.ForeColor = Color.White;
            lblMod.BackColor = Color.Transparent;
            lblMod.Font = new Font("Courier New", 9, FontStyle.Regular);
            lblMod.AutoSize = true;
            lblMod.Location = new Point(btnModDegistir.Left + btnModDegistir.Width + 5, btnModDegistir.Top + 5);

            if (aydinlikMod)
                lblMod.Text = "Aydınlık Mod";
            else
                lblMod.Text = "Karanlık Mod";

            // Eski label varsa sil, yenisini ekle
            var eskiLabel = this.Controls.Find("lblModYazisi", true).FirstOrDefault();
            if (eskiLabel != null)
                eskiLabel.Parent.Controls.Remove(eskiLabel);

            btnModDegistir.Parent.Controls.Add(lblMod);
        }






        private void btnParaGirisiniOnayla_Click(object sender, EventArgs e)
        {
            //  MessageBox.Show("buttona basıldı");
            KalanButceGuncelle();
            GiderGrafikGuncelle();
            KalanButceYuzdeGrafikCiz_Gelir();
            HaftalikGelirGrafikCiz();
            // MenuAltPaneli5SaatlikHarcamaTrend();
            try
            {
                // 1️⃣ Connection string
                string baglantiCumlesi = "Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;";

                using (SqlConnection baglanti = new SqlConnection(baglantiCumlesi))
                {
                    baglanti.Open();

                    // 2️⃣ Girilen tutarı al
                    decimal girilenTutar = decimal.Parse(textBox1.Text);

                    // 3️⃣ INSERT Gelirler tablosuna
                    SqlCommand komut = new SqlCommand(
                        "INSERT INTO Gelirler (KullaniciID, Tutar, Tarih) VALUES (@kid, @tutar, @tarih)",
                        baglanti);

                    komut.Parameters.AddWithValue("@kid", aktifKullaniciID); // DB’den gelen ID
                    komut.Parameters.AddWithValue("@tutar", girilenTutar);
                    komut.Parameters.AddWithValue("@tarih", DateTime.Now);

                    komut.ExecuteNonQuery();

                    // 4️⃣ Güncel toplam gelir hesapla
                    SqlCommand toplamKomut = new SqlCommand(
                        "SELECT SUM(Tutar) FROM Gelirler WHERE KullaniciID=@kid",
                        baglanti);

                    toplamKomut.Parameters.AddWithValue("@kid", aktifKullaniciID);
                    object sonuc = toplamKomut.ExecuteScalar();

                    decimal toplamGelir = (sonuc != DBNull.Value) ? Convert.ToDecimal(sonuc) : 0;

                    // 5️⃣ Label1’de göster
                    //  lblToplamButce.Text = "₺";
                    lblToplamButce.Text = "₺" + toplamGelir.ToString("N2"); // formatlı gösterim


                    textBox1.Clear();
                    KalanButceGuncelle();
                    GiderGrafikGuncelle();
                    HaftalikHarcamaGrafikCizHaftalik();
                    KalanButceYuzdeGrafikCiz_Gelir();
                    HaftalikGelirGrafikCiz();
                    // MenuAltPaneli5SaatlikHarcamaTrend();


                }
            }
            catch (FormatException)
            {
                MessageBox.Show("Lütfen geçerli bir tutar girin!", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private async void GiderAIYorumuBaslat()
        {
            try
            {
                decimal toplamHarcama = 0;
                string kategoriOzeti = "";

                using (SqlConnection baglanti = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
                {
                    baglanti.Open();

                    SqlCommand k1 = new SqlCommand("SELECT ISNULL(SUM(Tutar),0) FROM Harcamalar WHERE KullaniciID=@id", baglanti);
                    k1.Parameters.AddWithValue("@id", aktifKullaniciID);
                    toplamHarcama = Convert.ToDecimal(k1.ExecuteScalar());

                    SqlCommand k2 = new SqlCommand("SELECT TOP 3 Baslik, SUM(Tutar) as Toplam FROM Harcamalar WHERE KullaniciID=@id GROUP BY Baslik ORDER BY Toplam DESC", baglanti);
                    k2.Parameters.AddWithValue("@id", aktifKullaniciID);
                    SqlDataReader reader = k2.ExecuteReader();
                    while (reader.Read())
                        kategoriOzeti += $"{reader["Baslik"]}: {reader["Toplam"]:N2} TL, ";
                }

                string prompt = $"Kullanıcının toplam harcaması {toplamHarcama:N2} TL. En çok harcama yaptığı başlıklar: {kategoriOzeti}. Bu verilere göre Türkçe kısa bir harcama analizi yap ve gelecek ay için tahmin ve önerilerde bulun. 3-4 cümle.";

                string yorum = await AIAsistan.HarcamaYorumuAl(0, toplamHarcama);

                Label lblGiderAI = new Label();
                lblGiderAI.ForeColor = Color.Lime;
                lblGiderAI.BackColor = Color.Transparent;
                lblGiderAI.Font = new Font("Courier New", 10, FontStyle.Regular);
                lblGiderAI.AutoSize = false;
                lblGiderAI.Size = new Size(GiderAltPaneli5.Width - 20, GiderAltPaneli5.Height - 20);
                lblGiderAI.Location = new Point(10, 10);
                lblGiderAI.TextAlign = ContentAlignment.TopLeft;
                lblGiderAI.Text = "";

                GiderAltPaneli5.Controls.Clear();
                GiderAltPaneli5.Controls.Add(lblGiderAI);

                await YazmaEfekti(lblGiderAI, yorum);
            }
            catch (Exception ex)
            {
                MessageBox.Show("AI Hata: " + ex.Message);
            }
        }

        private void MenuAltPaneli1_Paint(object sender, PaintEventArgs e)
        {
            MenuAltPaneli1.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, MenuAltPaneli1.ClientRectangle,
                    Color.Lime, ButtonBorderStyle.Solid);
            };
        }

        private void GiderAltPaneli5_Paint(object sender, PaintEventArgs e)
        {
            GiderAltPaneli5.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, GiderAltPaneli5.ClientRectangle,
                    Color.Lime, ButtonBorderStyle.Solid);
            };
        }


        private void BildirimTimerBaslat()
        {
            bildirimTimer = new System.Windows.Forms.Timer();
            bildirimTimer.Interval = 10000; // 30 saniyede bir
            bildirimTimer.Tick += BildirimTimer_Tick;
            bildirimTimer.Start();
        }

        private void BildirimTimer_Tick(object sender, EventArgs e)
        {
            if (!bildirimAcik) return;

            var bildirimler = _bildirimService.OkunmamisBildirimleriGetir();

            foreach (var b in bildirimler)
            {
                if (b.BildirimID > _sonKontrolID)
                {
                    ToastHelper.Goster("ButceYonetimDB", b.Mesaj);
                    _sonKontrolID = b.BildirimID;
                    // BildirimPanelineEkle(b);
                }
            }
        }
        private void btnBildirim_Click(object sender, EventArgs e)
        {
            var bildirimler = _bildirimService.OkunmamisBildirimleriGetir();

            if (bildirimler.Count == 0)
            {
                MessageBox.Show("Okunmamış bildiriminiz yok.", "Bildirimler",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string mesajlar = "";
            foreach (var b in bildirimler)
            {
                mesajlar += $"[{b.Tarih:dd.MM.yyyy HH:mm}] {b.Mesaj}\n\n";
                _bildirimService.OkunduIsaretle(b.BildirimID);
            }

            MessageBox.Show(mesajlar, "Bildirimler",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnKisiselBilgiler_Click(object sender, EventArgs e)
        {
            pnlKisiselBilgiler.Visible = true;
            ProfilAltPaneli1.Visible = true;
            pnlSifreVeGüvenlik.Visible = false;
        }

        private void AnaPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void MenuAltPaneli2HareketGecmisiYukle()
        {
            MenuAltPaneli2.Controls.Clear();

            // Başlık etiketi
            Label lblBaslik = new Label();
            lblBaslik.Text = "📋 SON 1 AYLIK HAREKET GEÇMİŞİ";
            lblBaslik.Font = new Font("Courier New", 10, FontStyle.Bold);
            lblBaslik.ForeColor = Color.Lime;
            lblBaslik.BackColor = Color.Transparent;
            lblBaslik.AutoSize = false;
            lblBaslik.Size = new Size(MenuAltPaneli2.Width - 10, 30);
            lblBaslik.Location = new Point(5, 5);
            lblBaslik.TextAlign = ContentAlignment.MiddleLeft;
            MenuAltPaneli2.Controls.Add(lblBaslik);

            // Kaydırılabilir kart paneli
            FlowLayoutPanel akisPanel = new FlowLayoutPanel();
            akisPanel.FlowDirection = FlowDirection.TopDown;
            akisPanel.AutoScroll = true;
            akisPanel.Size = new Size(MenuAltPaneli2.Width - 10, MenuAltPaneli2.Height - 45);
            akisPanel.Location = new Point(5, 40);
            akisPanel.WrapContents = false;
            akisPanel.BackColor = Color.Transparent;
            MenuAltPaneli2.Controls.Add(akisPanel);

            try
            {
                using (SqlConnection baglanti = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
                {
                    baglanti.Open();

                    string sorgu = @"
                SELECT Tur, Tutar, Tarih, Aciklama FROM (
                    SELECT 'Gelir' AS Tur, Tutar, Tarih,
                        ISNULL(Aciklama, 'Para eklendi') AS Aciklama
                        FROM Gelirler
                        WHERE KullaniciID=@id AND Tarih >= DATEADD(MONTH,-1,GETDATE())
                    UNION ALL
                    SELECT 'Gider' AS Tur, Tutar, Tarih,
                        ISNULL(Baslik, 'Harcama yapıldı') AS Aciklama
                        FROM Harcamalar
                        WHERE KullaniciID=@id AND Tarih >= DATEADD(MONTH,-1,GETDATE())
                ) AS Tum
                ORDER BY Tarih DESC";

                    SqlCommand cmd = new SqlCommand(sorgu, baglanti);
                    cmd.Parameters.AddWithValue("@id", aktifKullaniciID);
                    SqlDataReader dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        string tur = dr["Tur"].ToString();
                        decimal tutar = Convert.ToDecimal(dr["Tutar"]);
                        DateTime tarih = Convert.ToDateTime(dr["Tarih"]);
                        string aciklama = dr["Aciklama"].ToString();

                        // Kart paneli
                        Panel kart = new Panel();
                        kart.Size = new Size(akisPanel.Width - 25, 65);
                        kart.BackColor = Color.FromArgb(20, 40, 60);
                        kart.Margin = new Padding(0, 0, 0, 6);

                        // Oval köşe
                        GraphicsPath gp = new GraphicsPath();
                        int r = 15;
                        gp.AddArc(0, 0, r, r, 180, 90);
                        gp.AddArc(kart.Width - r, 0, r, r, 270, 90);
                        gp.AddArc(kart.Width - r, kart.Height - r, r, r, 0, 90);
                        gp.AddArc(0, kart.Height - r, r, r, 90, 90);
                        gp.CloseFigure();
                        kart.Region = new Region(gp);

                        // Emoji + renk belirle
                        string emoji;
                        Color renk;
                        string mesaj;
                        if (tur == "Gelir")
                        {
                            emoji = "💰";
                            renk = Color.LimeGreen;
                            mesaj = $"+{tutar:N2} ₺  —  {aciklama}";
                        }
                        else
                        {
                            emoji = "💸";
                            renk = Color.Tomato;
                            mesaj = $"-{tutar:N2} ₺  —  {aciklama}";
                        }

                        // Sol renkli şerit
                        Panel serit = new Panel();
                        serit.Size = new Size(5, kart.Height);
                        serit.Location = new Point(0, 0);
                        serit.BackColor = renk;
                        kart.Controls.Add(serit);

                        // Emoji etiketi
                        Label lblEmoji = new Label();
                        lblEmoji.Text = emoji;
                        lblEmoji.Font = new Font("Segoe UI Emoji", 18);
                        lblEmoji.AutoSize = false;
                        lblEmoji.Size = new Size(40, 40);
                        lblEmoji.Location = new Point(15, 12);
                        lblEmoji.BackColor = Color.Transparent;
                        kart.Controls.Add(lblEmoji);

                        // İşlem açıklaması
                        Label lblMesaj = new Label();
                        lblMesaj.Text = mesaj;
                        lblMesaj.Font = new Font("Courier New", 9, FontStyle.Bold);
                        lblMesaj.ForeColor = renk;
                        lblMesaj.BackColor = Color.Transparent;
                        lblMesaj.AutoSize = false;
                        lblMesaj.Size = new Size(kart.Width - 75, 25);
                        lblMesaj.Location = new Point(60, 8);
                        lblMesaj.TextAlign = ContentAlignment.MiddleLeft;
                        kart.Controls.Add(lblMesaj);

                        // Tarih + saat
                        Label lblTarih = new Label();
                        lblTarih.Text = "🕐 " + tarih.ToString("dd.MM.yyyy  HH:mm");
                        lblTarih.Font = new Font("Courier New", 8, FontStyle.Regular);
                        lblTarih.ForeColor = Color.Silver;
                        lblTarih.BackColor = Color.Transparent;
                        lblTarih.AutoSize = false;
                        lblTarih.Size = new Size(kart.Width - 75, 20);
                        lblTarih.Location = new Point(60, 36);
                        kart.Controls.Add(lblTarih);

                        akisPanel.Controls.Add(kart);
                    }
                    dr.Close();
                }
            }
            catch (Exception ex)
            {
                Label lblHata = new Label();
                lblHata.Text = "Yüklenemedi: " + ex.Message;
                lblHata.ForeColor = Color.Red;
                lblHata.AutoSize = true;
                akisPanel.Controls.Add(lblHata);
            }
        }
        private void MenuAltPaneli4RozetYukle()
        {
            MenuAltPaneli4.Controls.Clear();

            // Başlık
            Label lblBaslik = new Label();
            lblBaslik.Text = "🏆 BU AYIN HARCAMA ŞAMPİYONU";
            lblBaslik.Font = new Font("Courier New", 10, FontStyle.Bold);
            lblBaslik.ForeColor = Color.Lime;
            lblBaslik.BackColor = Color.Transparent;
            lblBaslik.AutoSize = false;
            lblBaslik.Size = new Size(MenuAltPaneli4.Width - 10, 30);
            lblBaslik.Location = new Point(5, 5);
            lblBaslik.TextAlign = ContentAlignment.MiddleLeft;
            MenuAltPaneli4.Controls.Add(lblBaslik);

            try
            {
                using (SqlConnection baglanti = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
                {
                    baglanti.Open();

                    // En çok harcama yapılan kategoriyi bul
                    SqlCommand cmd = new SqlCommand(@"
                SELECT TOP 1 Baslik, SUM(Tutar) AS Toplam
                FROM Harcamalar
                WHERE KullaniciID=@id AND Tarih >= DATEADD(MONTH,-1,GETDATE())
                GROUP BY Baslik
                ORDER BY Toplam DESC", baglanti);
                    cmd.Parameters.AddWithValue("@id", aktifKullaniciID);

                    SqlDataReader dr = cmd.ExecuteReader();

                    if (dr.Read())
                    {
                        string baslik = dr["Baslik"].ToString().ToLower();
                        decimal toplam = Convert.ToDecimal(dr["Toplam"]);
                        dr.Close();

                        // Toplam harcama (yüzde hesabı için)
                        SqlCommand cmdToplam = new SqlCommand(
                            "SELECT ISNULL(SUM(Tutar),0) FROM Harcamalar WHERE KullaniciID=@id AND Tarih >= DATEADD(MONTH,-1,GETDATE())",
                            baglanti);
                        cmdToplam.Parameters.AddWithValue("@id", aktifKullaniciID);
                        decimal genelToplam = Convert.ToDecimal(cmdToplam.ExecuteScalar());
                        int yuzde = genelToplam > 0 ? (int)((toplam / genelToplam) * 100) : 0;

                        // Kategori emojisi
                        string emoji;
                        if (baslik.Contains("yemek") || baslik.Contains("restoran") || baslik.Contains("cafe"))
                            emoji = "🍔";
                        else if (baslik.Contains("market") || baslik.Contains("alışveriş"))
                            emoji = "🛒";
                        else if (baslik.Contains("ulaşım") || baslik.Contains("taksi") || baslik.Contains("otobüs"))
                            emoji = "🚗";
                        else if (baslik.Contains("eğlence") || baslik.Contains("sinema") || baslik.Contains("oyun"))
                            emoji = "🎮";
                        else if (baslik.Contains("fatura") || baslik.Contains("elektrik") || baslik.Contains("su"))
                            emoji = "💡";
                        else
                            emoji = "💰";

                        // Rozet seviyesi
                        string rozetSeviye;
                        Color rozetRenk;
                        Color rozetIcRenk;
                        if (toplam >= 5000)
                        {
                            rozetSeviye = "ALTIN";
                            rozetRenk = Color.Gold;
                            rozetIcRenk = Color.FromArgb(80, 60, 0);
                        }
                        else if (toplam >= 1000)
                        {
                            rozetSeviye = "GÜMÜŞ";
                            rozetRenk = Color.Silver;
                            rozetIcRenk = Color.FromArgb(50, 50, 60);
                        }
                        else
                        {
                            rozetSeviye = "BRONZ";
                            rozetRenk = Color.FromArgb(205, 127, 50);
                            rozetIcRenk = Color.FromArgb(60, 35, 10);
                        }

                        // Ana oval rozet kartı
                        Panel rozetKart = new Panel();
                        int kartW = MenuAltPaneli4.Width - 30;
                        int kartH = MenuAltPaneli4.Height - 60;
                        rozetKart.Size = new Size(kartW, kartH);
                        rozetKart.Location = new Point(15, 42);
                        rozetKart.BackColor = rozetIcRenk;

                        // Oval şekil
                        GraphicsPath gp = new GraphicsPath();
                        int r = 40;
                        gp.AddArc(0, 0, r, r, 180, 90);
                        gp.AddArc(kartW - r, 0, r, r, 270, 90);
                        gp.AddArc(kartW - r, kartH - r, r, r, 0, 90);
                        gp.AddArc(0, kartH - r, r, r, 90, 90);
                        gp.CloseFigure();
                        rozetKart.Region = new Region(gp);

                        // Parlayan kenarlık için Paint eventi
                        rozetKart.Paint += (s, e) =>
                        {
                            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                            using (Pen pen = new Pen(rozetRenk, 3))
                            {
                                e.Graphics.DrawPath(pen, gp);
                            }
                        };

                        // Büyük emoji
                        Label lblEmoji = new Label();
                        lblEmoji.Text = emoji;
                        lblEmoji.Font = new Font("Segoe UI Emoji", 36);
                        lblEmoji.AutoSize = false;
                        lblEmoji.Size = new Size(80, 80);
                        lblEmoji.Location = new Point(kartW / 2 - 40, 20);
                        lblEmoji.BackColor = Color.Transparent;
                        lblEmoji.TextAlign = ContentAlignment.MiddleCenter;
                        rozetKart.Controls.Add(lblEmoji);

                        // Rozet seviye yazısı
                        Label lblSeviye = new Label();
                        lblSeviye.Text = "✨ " + rozetSeviye + " ROZETİ ✨";
                        lblSeviye.Font = new Font("Courier New", 11, FontStyle.Bold);
                        lblSeviye.ForeColor = rozetRenk;
                        lblSeviye.BackColor = Color.Transparent;
                        lblSeviye.AutoSize = false;
                        lblSeviye.Size = new Size(kartW - 20, 25);
                        lblSeviye.Location = new Point(10, 110);
                        lblSeviye.TextAlign = ContentAlignment.MiddleCenter;
                        rozetKart.Controls.Add(lblSeviye);

                        // Kategori adı
                        Label lblKategori = new Label();
                        lblKategori.Text = dr.IsClosed ? baslik.ToUpper() : baslik.ToUpper();
                        lblKategori.Font = new Font("Courier New", 13, FontStyle.Bold);
                        lblKategori.ForeColor = Color.White;
                        lblKategori.BackColor = Color.Transparent;
                        lblKategori.AutoSize = false;
                        lblKategori.Size = new Size(kartW - 20, 30);
                        lblKategori.Location = new Point(10, 140);
                        lblKategori.TextAlign = ContentAlignment.MiddleCenter;
                        rozetKart.Controls.Add(lblKategori);

                        // Tutar
                        Label lblTutar = new Label();
                        lblTutar.Text = toplam.ToString("N2") + " ₺";
                        lblTutar.Font = new Font("Courier New", 12, FontStyle.Bold);
                        lblTutar.ForeColor = rozetRenk;
                        lblTutar.BackColor = Color.Transparent;
                        lblTutar.AutoSize = false;
                        lblTutar.Size = new Size(kartW - 20, 25);
                        lblTutar.Location = new Point(10, 175);
                        lblTutar.TextAlign = ContentAlignment.MiddleCenter;
                        rozetKart.Controls.Add(lblTutar);

                        // Yüzde çubuğu arkaplan
                        Panel barArka = new Panel();
                        barArka.Size = new Size(kartW - 40, 12);
                        barArka.Location = new Point(20, 210);
                        barArka.BackColor = Color.FromArgb(40, 40, 40);
                        GraphicsPath barGp = new GraphicsPath();
                        barGp.AddArc(0, 0, 12, 12, 180, 90);
                        barGp.AddArc(barArka.Width - 12, 0, 12, 12, 270, 90);
                        barGp.AddArc(barArka.Width - 12, barArka.Height - 12, 12, 12, 0, 90);
                        barGp.AddArc(0, barArka.Height - 12, 12, 12, 90, 90);
                        barGp.CloseFigure();
                        barArka.Region = new Region(barGp);
                        rozetKart.Controls.Add(barArka);

                        // Yüzde çubuğu dolgu
                        int doluGenislik = Math.Max(12, (int)((kartW - 40) * yuzde / 100.0));
                        Panel barDolu = new Panel();
                        barDolu.Size = new Size(doluGenislik, 12);
                        barDolu.Location = new Point(0, 0);
                        barDolu.BackColor = rozetRenk;
                        GraphicsPath barDoluGp = new GraphicsPath();
                        barDoluGp.AddArc(0, 0, 12, 12, 180, 90);
                        barDoluGp.AddArc(barDolu.Width - 12, 0, 12, 12, 270, 90);
                        barDoluGp.AddArc(barDolu.Width - 12, barDolu.Height - 12, 12, 12, 0, 90);
                        barDoluGp.AddArc(0, barDolu.Height - 12, 12, 12, 90, 90);
                        barDoluGp.CloseFigure();
                        barDolu.Region = new Region(barDoluGp);
                        barArka.Controls.Add(barDolu);

                        // Yüzde yazısı
                        Label lblYuzde = new Label();
                        lblYuzde.Text = $"Toplam harcamanın %{yuzde}'i";
                        lblYuzde.Font = new Font("Courier New", 8, FontStyle.Regular);
                        lblYuzde.ForeColor = Color.Silver;
                        lblYuzde.BackColor = Color.Transparent;
                        lblYuzde.AutoSize = false;
                        lblYuzde.Size = new Size(kartW - 20, 20);
                        lblYuzde.Location = new Point(10, 228);
                        lblYuzde.TextAlign = ContentAlignment.MiddleCenter;
                        rozetKart.Controls.Add(lblYuzde);

                        MenuAltPaneli4.Controls.Add(rozetKart);
                    }
                    else
                    {
                        dr.Close();
                        Label lblBos = new Label();
                        lblBos.Text = "Henüz harcama verisi yok 😊";
                        lblBos.ForeColor = Color.Silver;
                        lblBos.Font = new Font("Courier New", 10);
                        lblBos.AutoSize = true;
                        lblBos.Location = new Point(20, 60);
                        lblBos.BackColor = Color.Transparent;
                        MenuAltPaneli4.Controls.Add(lblBos);
                    }
                }
            }
            catch (Exception ex)
            {
                Label lblHata = new Label();
                lblHata.Text = "Hata: " + ex.Message;
                lblHata.ForeColor = Color.Red;
                lblHata.AutoSize = true;
                lblHata.Location = new Point(10, 50);
                MenuAltPaneli4.Controls.Add(lblHata);
            }
        }

        private void MenuAltPaneli3TermometreYukle()
        {
            MenuAltPaneli3.Controls.Clear();

            // Başlık
            Label lblBaslik = new Label();
            lblBaslik.Text = "🌡️ BÜTÇE TERMOMETRESİ";
            lblBaslik.Font = new Font("Courier New", 10, FontStyle.Bold);
            lblBaslik.ForeColor = Color.Lime;
            lblBaslik.BackColor = Color.Transparent;
            lblBaslik.AutoSize = false;
            lblBaslik.Size = new Size(362, 30);
            lblBaslik.Location = new Point(5, 8);
            lblBaslik.TextAlign = ContentAlignment.MiddleLeft;
            MenuAltPaneli3.Controls.Add(lblBaslik);

            try
            {
                using (SqlConnection baglanti = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;"))
                {
                    baglanti.Open();

                    SqlCommand cmdGelir = new SqlCommand(
                        "SELECT ISNULL(SUM(Tutar),0) FROM Gelirler WHERE KullaniciID=@id AND Tarih >= DATEADD(MONTH,-1,GETDATE())",
                        baglanti);
                    cmdGelir.Parameters.AddWithValue("@id", aktifKullaniciID);
                    decimal gelir = Convert.ToDecimal(cmdGelir.ExecuteScalar());

                    SqlCommand cmdHarcama = new SqlCommand(
                        "SELECT ISNULL(SUM(Tutar),0) FROM Harcamalar WHERE KullaniciID=@id AND Tarih >= DATEADD(MONTH,-1,GETDATE())",
                        baglanti);
                    cmdHarcama.Parameters.AddWithValue("@id", aktifKullaniciID);
                    decimal harcama = Convert.ToDecimal(cmdHarcama.ExecuteScalar());

                    double yuzde = gelir > 0 ? Math.Min((double)(harcama / gelir) * 100.0, 100.0) : 100.0;
                    decimal kalan = gelir - harcama;

                    Color termRenk;
                    string durumYazi;
                    if (yuzde < 50)
                    {
                        termRenk = Color.LimeGreen;
                        durumYazi = "✅ Bütçen sağlıklı!";
                    }
                    else if (yuzde < 80)
                    {
                        termRenk = Color.Orange;
                        durumYazi = "⚠️ Dikkatli harca!";
                    }
                    else
                    {
                        termRenk = Color.Tomato;
                        durumYazi = "🚨 Bütçe tehlikede!";
                    }

                    // --- TERMOMETRE KONUMLARI ---
                    int termX = 170;       // termometrenin sol x konumu
                    int termUstY = 45;     // gövdenin başladığı y
                    int govdeW = 22;       // gövde genişliği
                    int govdeH = 230;      // gövde yüksekliği
                    int balonCap = 50;     // alt balon çapı
                    int balonX = termX - (balonCap - govdeW) / 2;
                    int balonY = termUstY + govdeH;

                    // Gövde arka plan (gri)
                    Panel govdeArka = new Panel();
                    govdeArka.Size = new Size(govdeW, govdeH);
                    govdeArka.Location = new Point(termX, termUstY);
                    govdeArka.BackColor = Color.FromArgb(50, 50, 60);
                    GraphicsPath gpGovde = new GraphicsPath();
                    gpGovde.AddArc(0, 0, govdeW, govdeW, 180, 180); // üst yarım daire
                    gpGovde.AddLine(0, govdeW / 2, 0, govdeH);
                    gpGovde.AddLine(0, govdeH, govdeW, govdeH);
                    gpGovde.AddLine(govdeW, govdeH, govdeW, govdeW / 2);
                    gpGovde.CloseFigure();
                    govdeArka.Region = new Region(gpGovde);
                    MenuAltPaneli3.Controls.Add(govdeArka);

                    // Gövde dolgu (renkli, aşağıdan yukarı)
                    int dolguH = (int)(govdeH * yuzde / 100.0);
                    int dolguY = govdeH - dolguH;

                    Panel govdeDolu = new Panel();
                    govdeDolu.Size = new Size(govdeW, dolguH);
                    govdeDolu.Location = new Point(0, dolguY);
                    govdeDolu.BackColor = termRenk;
                    govdeArka.Controls.Add(govdeDolu);

                    // Alt balon arka plan (gri)
                    Panel balonArka = new Panel();
                    balonArka.Size = new Size(balonCap, balonCap);
                    balonArka.Location = new Point(balonX, balonY);
                    balonArka.BackColor = Color.FromArgb(50, 50, 60);
                    GraphicsPath gpBalon = new GraphicsPath();
                    gpBalon.AddEllipse(0, 0, balonCap, balonCap);
                    balonArka.Region = new Region(gpBalon);
                    MenuAltPaneli3.Controls.Add(balonArka);

                    // Alt balon renkli
                    Panel balonDolu = new Panel();
                    balonDolu.Size = new Size(balonCap, balonCap);
                    balonDolu.Location = new Point(0, 0);
                    balonDolu.BackColor = termRenk;
                    GraphicsPath gpBalonDolu = new GraphicsPath();
                    gpBalonDolu.AddEllipse(0, 0, balonCap, balonCap);
                    balonDolu.Region = new Region(gpBalonDolu);
                    balonArka.Controls.Add(balonDolu);

                    // Yüzde işaretleri (sağ tarafa)
                    string[] etiketler = { "%100", "%75", "%50", "%25", "%0" };
                    for (int i = 0; i < etiketler.Length; i++)
                    {
                        Label lblTick = new Label();
                        lblTick.Text = etiketler[i];
                        lblTick.Font = new Font("Courier New", 8);
                        lblTick.ForeColor = Color.Silver;
                        lblTick.BackColor = Color.Transparent;
                        lblTick.AutoSize = true;
                        lblTick.Location = new Point(termX + govdeW + 8, termUstY + (int)(govdeH * i / 4.0) - 8);
                        MenuAltPaneli3.Controls.Add(lblTick);
                    }

                    // Çizgi işaretleri
                    for (int i = 0; i < 5; i++)
                    {
                        Panel cizgi = new Panel();
                        cizgi.Size = new Size(6, 2);
                        cizgi.BackColor = Color.Silver;
                        cizgi.Location = new Point(termX + govdeW, termUstY + (int)(govdeH * i / 4.0));
                        MenuAltPaneli3.Controls.Add(cizgi);
                    }

                    // Durum yazısı
                    Label lblDurum = new Label();
                    lblDurum.Text = durumYazi;
                    lblDurum.Font = new Font("Courier New", 11, FontStyle.Bold);
                    lblDurum.ForeColor = termRenk;
                    lblDurum.BackColor = Color.Transparent;
                    lblDurum.AutoSize = false;
                    lblDurum.Size = new Size(362, 28);
                    lblDurum.Location = new Point(5, 355);
                    lblDurum.TextAlign = ContentAlignment.MiddleCenter;
                    MenuAltPaneli3.Controls.Add(lblDurum);

                    // Gelir
                    Label lblGelir = new Label();
                    lblGelir.Text = $"💰 Gelir:   {gelir:N2} ₺";
                    lblGelir.Font = new Font("Courier New", 9);
                    lblGelir.ForeColor = Color.LimeGreen;
                    lblGelir.BackColor = Color.Transparent;
                    lblGelir.AutoSize = false;
                    lblGelir.Size = new Size(362, 22);
                    lblGelir.Location = new Point(5, 388);
                    lblGelir.TextAlign = ContentAlignment.MiddleCenter;
                    MenuAltPaneli3.Controls.Add(lblGelir);

                    // Harcama
                    Label lblHarcama = new Label();
                    lblHarcama.Text = $"💸 Harcama: {harcama:N2} ₺";
                    lblHarcama.Font = new Font("Courier New", 9);
                    lblHarcama.ForeColor = Color.Tomato;
                    lblHarcama.BackColor = Color.Transparent;
                    lblHarcama.AutoSize = false;
                    lblHarcama.Size = new Size(362, 22);
                    lblHarcama.Location = new Point(5, 412);
                    lblHarcama.TextAlign = ContentAlignment.MiddleCenter;
                    MenuAltPaneli3.Controls.Add(lblHarcama);

                    // Kalan
                    Label lblKalan = new Label();
                    lblKalan.Text = $"🏦 Kalan:   {kalan:N2} ₺";
                    lblKalan.Font = new Font("Courier New", 9, FontStyle.Bold);
                    lblKalan.ForeColor = Color.White;
                    lblKalan.BackColor = Color.Transparent;
                    lblKalan.AutoSize = false;
                    lblKalan.Size = new Size(362, 22);
                    lblKalan.Location = new Point(5, 436);
                    lblKalan.TextAlign = ContentAlignment.MiddleCenter;
                    MenuAltPaneli3.Controls.Add(lblKalan);
                }
            }
            catch (Exception ex)
            {
                Label lblHata = new Label();
                lblHata.Text = "Hata: " + ex.Message;
                lblHata.ForeColor = Color.Red;
                lblHata.AutoSize = true;
                lblHata.Location = new Point(10, 50);
                MenuAltPaneli3.Controls.Add(lblHata);
            }
        }

        private void btnpnlKisiselBilgilerDegistir_Click(object sender, EventArgs e)
        {
            string yeniKullaniciAdi = KA.Text.Trim();
            string yeniTelefon = TN.Text.Trim();

            if (string.IsNullOrEmpty(yeniKullaniciAdi) && string.IsNullOrEmpty(yeniTelefon))
            {
                MessageBox.Show("Lütfen en az bir alanı doldurun.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string baglantiCumlesi = "Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;";

                using (SqlConnection con = new SqlConnection(baglantiCumlesi))
                {
                    con.Open();

                    List<string> guncellemeler = new List<string>();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = con;

                    if (!string.IsNullOrEmpty(yeniKullaniciAdi))
                    {
                        guncellemeler.Add("KullaniciAdi = @kullaniciAdi");
                        cmd.Parameters.AddWithValue("@kullaniciAdi", yeniKullaniciAdi);
                    }

                    if (!string.IsNullOrEmpty(yeniTelefon))
                    {
                        guncellemeler.Add("TelefonNumarasi = @telefon");
                        cmd.Parameters.AddWithValue("@telefon", yeniTelefon);
                    }

                    cmd.CommandText = "UPDATE Kullanicilar SET " + string.Join(", ", guncellemeler) + " WHERE KullaniciID = @id";
                    cmd.Parameters.AddWithValue("@id", aktifKullaniciID);

                    int etkilenen = cmd.ExecuteNonQuery();

                    if (etkilenen > 0)
                    {
                        MessageBox.Show("Bilgiler başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        KA.Text = "";
                        TN.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Güncelleme başarısız.\nAktif ID: " + aktifKullaniciID, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnPnlKisiselBilgilerGeri_Click(object sender, EventArgs e)
        {
            pnlKisiselBilgiler.Visible = false;

        }

        private void btnSifreVeGuvenlik_Click(object sender, EventArgs e)
        {
            pnlSifreVeGüvenlik.Visible = true;
            pnlKisiselBilgiler.Visible = false;
            //pnlZilSesleri.Visible = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            pnlSifreVeGüvenlik.Visible = false;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string yeniSifre = textBox9.Text.Trim();

            if (string.IsNullOrEmpty(yeniSifre))
            {
                MessageBox.Show("Lütfen yeni şifreyi girin.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string baglantiCumlesi = "Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;";

                using (SqlConnection con = new SqlConnection(baglantiCumlesi))
                {
                    con.Open();

                    SqlCommand cmd = new SqlCommand(
                        "UPDATE Kullanicilar SET Sifre = @sifre WHERE KullaniciID = @id", con);

                    cmd.Parameters.AddWithValue("@sifre", yeniSifre);
                    cmd.Parameters.AddWithValue("@id", aktifKullaniciID);

                    int etkilenen = cmd.ExecuteNonQuery();

                    if (etkilenen > 0)
                    {
                        MessageBox.Show("Şifre başarıyla güncellendi.", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBox9.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Güncelleme başarısız.\nAktif ID: " + aktifKullaniciID, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata: " + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnZilSesiDegis_Click(object sender, EventArgs e)
        {
            // pnlZilSesleri.Visible = true;
        }

        private void button9_Click(object sender, EventArgs e)
        {
            pnlSohbetEt.Visible = false;
            dgvArkadaslar.Visible = false;
            dgvIstekler.Visible = false;

            dgvKullanicilar.Visible = true;
        }

        private void button10_Click(object sender, EventArgs e)
        {

            dgvIstekler.Visible = false;
            dgvKullanicilar.Visible = false;

            dgvArkadaslar.Visible = true;
            pnlSohbetEt.Visible = true;
        }

        private void button11_Click(object sender, EventArgs e)
        {
            pnlSohbetEt.Visible = false;
            dgvArkadaslar.Visible = false;
            dgvIstekler.Visible = true;

            dgvKullanicilar.Visible = false;
        }
    }
}



