using FitnessCenterManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenterManagement.Controllers
{
    public class AiController : Controller
    {
        private readonly GeminiAiService _geminiAiService;

        public AiController(GeminiAiService geminiAiService)
        {
            _geminiAiService = geminiAiService;
        }

        // FORM
        public IActionResult Index()
        {
            return View();
        }

        // FORM SUBMIT
        [HttpPost]
        public async Task<IActionResult> Index(
            int heightCm,
            int weightKg,
            string goal)
        {
            var result = await _geminiAiService
                .GenerateWorkoutAndDietPlan(heightCm, weightKg, goal);

            ViewBag.Result = result;
            return View();
        }
    }
}
