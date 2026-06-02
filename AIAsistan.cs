using System;
using System.Net.Http;
using System.Text;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _2.sınıf_2._dönem_projesi
{
    internal class AIAsistan
    {
        private static readonly HttpClient istemci = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

      
        private const string API_KEY = "";
        public static async Task<string> HarcamaYorumuAl(decimal toplamGelir, decimal toplamHarcama)
        {
            try
            {
                decimal kalan = toplamGelir - toplamHarcama;

                string prompt = $"Sen bir kişisel finans asistanısın. Kullanıcının bütçe durumu: Toplam Gelir: {toplamGelir:N2} TL, Toplam Harcama: {toplamHarcama:N2} TL, Kalan Bakiye: {kalan:N2} TL. Yanıtına mutlaka 'Merhaba!' diye başla, sonra kısa, samimi ve motive edici bir Türkçe yorum yap. 2-3 cümle yeterli.";

                var requestBody = new
                {
                    model = "llama-3.1-8b-instant",
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    }
                };

                string json = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);
                var icerik = new StringContent(json, Encoding.UTF8, "application/json");

                istemci.DefaultRequestHeaders.Clear();
                istemci.DefaultRequestHeaders.Add("Authorization", $"Bearer {API_KEY}");

                var yanit = await istemci.PostAsync(
                    "https://api.groq.com/openai/v1/chat/completions", icerik);

                string yanitMetin = await yanit.Content.ReadAsStringAsync();
               // MessageBox.Show("Sunucu yanıtı: " + yanitMetin);
                if (!yanit.IsSuccessStatusCode)
                    return "AI şu an kullanılamıyor.";

                var jObj = Newtonsoft.Json.Linq.JObject.Parse(yanitMetin);
                return jObj["choices"]?[0]?["message"]?["content"]?.ToString()
                    ?? "AI şu an kullanılamıyor.";
            }
            catch (Exception ex)
            {
                MessageBox.Show("AI Hata: " + ex.Message);
                return "AI şu an kullanılamıyor.";
            }
        }
    }
}