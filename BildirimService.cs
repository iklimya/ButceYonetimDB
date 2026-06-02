using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace _2.sınıf_2._dönem_projesi
{
    public class Bildirim
    {
        public int BildirimID { get; set; }
        public string Mesaj { get; set; }
        public DateTime Tarih { get; set; }
    }

    public class BildirimService
    {
        private readonly string _connStr = "Server=(localdb)\\MSSQLLocalDB;Database=ButceYonetimDB;Trusted_Connection=True;";
        private readonly int _kullaniciID;

        public BildirimService(int kullaniciID)
        {
            _kullaniciID = kullaniciID;
        }

        // Okunmamış bildirimleri getir
        public List<Bildirim> OkunmamisBildirimleriGetir()
        {
            var liste = new List<Bildirim>();
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT BildirimID, Mesaj, Tarih FROM Bildirimlerim " +
                    "WHERE KullaniciID = @kid AND Okundu = 0 ORDER BY Tarih DESC", conn);
                cmd.Parameters.AddWithValue("@kid", _kullaniciID);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        liste.Add(new Bildirim
                        {
                            BildirimID = reader.GetInt32(0),
                            Mesaj = reader.GetString(1),
                            Tarih = reader.GetDateTime(2)
                        });
                    }
                }
            }
            return liste;
        }

        // Bildirimi okundu işaretle
        public void OkunduIsaretle(int bildirimID)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "UPDATE Bildirimlerim SET Okundu = 1 WHERE BildirimID = @id", conn);
                cmd.Parameters.AddWithValue("@id", bildirimID);
                cmd.ExecuteNonQuery();
            }
        }

        // Manuel bildirim ekle
        public void ManuelBildirimEkle(string mesaj)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "INSERT INTO Bildirimlerim (KullaniciID, Mesaj, Tarih, Okundu) " +
                    "VALUES (@kid, @mesaj, GETDATE(), 0)", conn);
                cmd.Parameters.AddWithValue("@kid", _kullaniciID);
                cmd.Parameters.AddWithValue("@mesaj", mesaj);
                cmd.ExecuteNonQuery();
            }
        }

        // Bütçe doluluk oranı kontrolü
        public void ButceDolulukKontrol(decimal aylikLimit)
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT ISNULL(SUM(Tutar), 0) FROM Harcamalar " +
                    "WHERE KullaniciID = @kid " +
                    "AND MONTH(Tarih) = MONTH(GETDATE()) " +
                    "AND YEAR(Tarih) = YEAR(GETDATE())", conn);
                cmd.Parameters.AddWithValue("@kid", _kullaniciID);

                decimal toplam = (decimal)cmd.ExecuteScalar();
                decimal oran = (aylikLimit > 0) ? (toplam / aylikLimit) * 100 : 0;

                if (oran >= 80 && oran < 100)
                    ManuelBildirimEkle($"Dikkat! Aylık bütçenizin %{oran:F0}'ini kullandınız.");
                else if (oran >= 100)
                    ManuelBildirimEkle($"Aylık bütçe limitinizi aştınız! (%{oran:F0})");
            }
        }

        // Okunmamış bildirim sayısını getir
        public int OkunmamisSayisiGetir()
        {
            using (var conn = new SqlConnection(_connStr))
            {
                conn.Open();
                var cmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Bildirimlerim " +
                    "WHERE KullaniciID = @kid AND Okundu = 0", conn);
                cmd.Parameters.AddWithValue("@kid", _kullaniciID);
                return (int)cmd.ExecuteScalar();
            }
        }
    }
}