using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FitnessCenterManagement.Controllers
{
    public class AiController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(int height, int weight, string goal)
        {
            // 1️⃣ BASİT EGZERSİZ ÖNERİSİ (MINIMUM)
            string plan = goal switch
            {
                "zayıflama" => "Haftada 4 gün kardiyo, yürüyüş ve plank egzersizleri önerilir.",
                "kas" => "Ağırlık çalışmaları, squat, bench press ve protein ağırlıklı beslenme önerilir.",
                _ => "Dengeli kardiyo ve vücut ağırlığı egzersizleri önerilir."
            };

            ViewBag.Plan = plan;

            // 2️⃣ YAPAY ZEKA GÖRSELİ
            string prompt =
                $"realistic fitness body, {goal}, height {height}cm, weight {weight}kg, gym, high quality photo";

            ViewBag.ImageUrl =
                $"https://image.pollinations.ai/prompt/{WebUtility.UrlEncode(prompt)}";

            return View();
        }
    }
}
