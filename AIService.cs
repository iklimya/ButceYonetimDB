using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace _2.sınıf_2._dönem_projesi
{
    internal class AIService
    {
     
       private string apiKey = "";

        public async Task<string> AnalizYap(string veri)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                var requestBody = new
                {
                    model = "llama-3.1-8b-instant",
                    messages = new[]
                    {
                        new
                        {
                            role = "user",
                            content = "Sen bir finans uzmanısın. Türkçe yanıt ver: " + veri
                        }
                    }
                };

                string json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync(
                    "https://api.groq.com/openai/v1/chat/completions",
                    content
                );

                string result = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                    return "API Hatası: " + result;

                var jsonObj = JObject.Parse(result);
                string metin = jsonObj["choices"]?[0]?["message"]?["content"]?.ToString();

                return metin ?? "Yanıt alınamadı.";
            }
        }
    }
}