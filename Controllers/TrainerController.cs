using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        private async Task<List<SelectListItem>> GetGymsDropdown()
        {
            return await _context.Gyms
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                })
                .ToListAsync();
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var trainers = await _context.Trainers
                .Include(t => t.Gym)
                .ToListAsync();

            return View(trainers);
        }

        // CREATE GET
        public async Task<IActionResult> Create()
        {
            ViewBag.Gyms = await _context.Gyms
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                })
                .ToListAsync();

            return View();
        }


        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Create(Trainer trainer)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Gyms = await _context.Gyms
                    .Select(g => new SelectListItem
                    {
                        Value = g.Id.ToString(),
                        Text = g.Name
                    })
                    .ToListAsync();

                return View(trainer);
            }

            _context.Trainers.Add(trainer);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
                return NotFound();

            ViewBag.Gyms = await GetGymsDropdown();
            return View(trainer);
        }

        // EDIT POST
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Trainer trainer)
        {
            if (id != trainer.Id)
                return NotFound();


            if (!ModelState.IsValid)
            {
                ViewBag.Gyms = await GetGymsDropdown();
                return View(trainer);
            }

            var existing = await _context.Trainers.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.FirstName = trainer.FirstName;
            existing.LastName = trainer.LastName;
            existing.Specializations = trainer.Specializations;
            existing.IsActive = trainer.IsActive;
            existing.GymId = trainer.GymId; // 🔥 GYM GÜNCELLENİYOR

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
                return NotFound();

            // Eğer randevular bağlıysa silmeyi engelle
            var hasAppointments = await _context.Appointments
                .AnyAsync(a => a.TrainerId == id);

            if (hasAppointments)
            {
                ViewBag.Error = "Bu eğitmen randevulara bağlı olduğu için silinemez.";
                return View(trainer);
            }

            return View(trainer);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            TempData["Error"] = "Bu eğitmen randevulara bağlı olduğu için silinemez.";
            return RedirectToAction(nameof(Index));
        }

    }
}
