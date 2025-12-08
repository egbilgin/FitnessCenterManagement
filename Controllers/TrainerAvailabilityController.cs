using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TrainerAvailabilityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainerAvailabilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int trainerId)
        {
            var trainer = await _context.Trainers
                .Include(t => t.Availabilities)
                .FirstOrDefaultAsync(t => t.Id == trainerId);

            if (trainer == null)
                return NotFound();

            ViewBag.Trainer = trainer;
            return View(trainer.Availabilities.ToList());
        }

        public IActionResult Create(int trainerId)
        {
            var availability = new TrainerAvailability
            {
                TrainerId = trainerId
            };

            return View(availability);
        }

        [HttpPost]
        public async Task<IActionResult> Create(TrainerAvailability availability)
        {
            if (!ModelState.IsValid)
                return View(availability);

            _context.TrainerAvailabilities.Add(availability);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { trainerId = availability.TrainerId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var a = await _context.TrainerAvailabilities.FindAsync(id);
            if (a == null)
                return NotFound();

            return View(a);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var availability = await _context.TrainerAvailabilities.FindAsync(id);

            if (availability != null)
            {
                _context.TrainerAvailabilities.Remove(availability);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { trainerId = availability.TrainerId });
        }
    }
}
