using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrainerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var trainers = await _context.Trainers.ToListAsync();
            return View(trainers);
        }

        // CREATE GET
        public IActionResult Create()
        {
            return View();
        }

        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Create(Trainer trainer)
        {

            // Appointments listesi formdan gelmediği için validasyon hatası veriyorsa, o hatayı görmezden gel diyoruz.
            ModelState.Remove("Appointments");

            if (ModelState.IsValid)
            {
                _context.Trainers.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer == null)
                return NotFound();

            return View(trainer);
        }

        // EDIT POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Trainer trainer)
        {
            if (!ModelState.IsValid)
            {
                return View(trainer);
            }

            var existing = await _context.Trainers.FindAsync(trainer.Id);

            if (existing == null)
                return NotFound();

            existing.FirstName = trainer.FirstName;
            existing.LastName = trainer.LastName;
            existing.Specializations = trainer.Specializations;
            existing.IsActive = trainer.IsActive;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer == null)
                return NotFound();

            return View(trainer);
        }

        // DELETE POST CONFIRM
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer == null)
                return NotFound();

            // Bu trainer'a bağlı randevu var mı?
            bool hasAppointments = await _context.Appointments
                .AnyAsync(a => a.TrainerId == id);

            if (hasAppointments)
            {
                TempData["Error"] = "Bu antrenör silinemiyor çünkü ona bağlı randevular bulunmaktadır!";
                return RedirectToAction(nameof(Index));
            }

            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

    }
}
