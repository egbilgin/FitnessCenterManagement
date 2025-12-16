using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration; // Bunu unutma

namespace FitnessCenterManagement.Services
{
    public class GeminiAiService
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public GeminiAiService(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
        }

        public async Task<string> GenerateWorkoutAndDietPlan(int heightCm, int weightKg, string goal)
        {
            // -----------------------------------------------------------------------
            // DÜZELTME BURADA:
            // JSON'daki "Gemini" başlığının altındaki "ApiKey" değerini okuyoruz.
            // -----------------------------------------------------------------------
            var apiKey = _configuration["Gemini:ApiKey"];

            // Eğer API Key okunamazsa hata verelim ki sorunu anlayalım
            if (string.IsNullOrEmpty(apiKey))
            {
                return "HATA: API Anahtarı appsettings.json dosyasından okunamadı. 'Gemini:ApiKey' ayarını kontrol edin.";
            }

            var url =
                $"https://generativelanguage.googleapis.com/v1/models/gemini-1.5-flash-latest:generateContent?key={apiKey}";


            var prompt = $@"
Boyum {heightCm} cm, kilom {weightKg} kg.
Hedefim: {goal}.

Bana şunları hazırla:
1. Bir günlük örnek beslenme programı (Sabah, Öğle, Akşam)
2. Yapmam gereken 3 temel egzersiz
Cevabın kısa, maddeler halinde ve Türkçe olsun.
";

            // Google Gemini JSON Formatı
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync(url, content);

                // HTTP Hatası varsa yakala (Örn: 400, 404, 500)
                if (!response.IsSuccessStatusCode)
                {
                    var errorMsg = await response.Content.ReadAsStringAsync();
                    return $"API Bağlantı Hatası ({response.StatusCode}): {errorMsg}";
                }

                var responseJson = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(responseJson);

                // Dönen JSON'ı güvenli şekilde çözümle
                if (doc.RootElement.TryGetProperty("candidates", out var candidates) && candidates.GetArrayLength() > 0)
                {
                    var text = candidates[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text")
                        .GetString();

                    return text;
                }

                return "Yapay zeka boş bir cevap döndürdü.";
            }
            catch (Exception ex)
            {
                return $"Sistemsel Hata: {ex.Message}";
            }
        }
    }
}