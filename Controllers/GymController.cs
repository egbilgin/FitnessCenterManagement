using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GymController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GymController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST — Tek bir Gym varsa onu göster, yoksa "ekleyin" uyarısı göster
        public async Task<IActionResult> Index()
        {
            var gyms = await _context.Gyms.ToListAsync();
            return View(gyms);
        }

        // CREATE GET
        public IActionResult Create()
        {
            return View();
        }

        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Create(Gym gym)
        {
            // Eğer zaten 1 Gym varsa, yenisini eklemeyi engelle
            if (_context.Gyms.Count() >= 1)
            {
                ModelState.AddModelError("", "Sistemde zaten bir salon mevcut. Yeni salon ekleyemezsiniz.");
                return View(gym);
            }

            if (ModelState.IsValid)
            {
                _context.Gyms.Add(gym);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(gym);
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var gym = await _context.Gyms.FindAsync(id);
            if (gym == null)
                return NotFound();

            return View(gym);
        }

        // EDIT POST
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Gym gym)
        {
            if (id != gym.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(gym);

            var existing = await _context.Gyms.FindAsync(id);
            if (existing == null)
                return NotFound();

            // Güncellenebilecek alanlar:
            existing.Name = gym.Name;
            existing.Address = gym.Address;
            existing.OpeningTime = gym.OpeningTime;
            existing.ClosingTime = gym.ClosingTime;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE GET — Kullanıcıya silinemeyeceğini açıkça göster
        public async Task<IActionResult> Delete(int id)
        {
            var gym = await _context.Gyms.FindAsync(id);
            if (gym == null)
                return NotFound();

            ViewBag.Error = "Salon silinemez. Sistemde en az 1 salon bulunmak zorundadır.";
            return View(gym);
        }

        // DELETE CONFIRM — SİLMEYİ ENGELLİYORUZ
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            TempData["Error"] = "Salon silinemez. Sistemde en az 1 salon bulunmak zorundadır.";
            TempData["ErrorSource"] = "Gym";
            return RedirectToAction(nameof(Index));
        }
    }
}
