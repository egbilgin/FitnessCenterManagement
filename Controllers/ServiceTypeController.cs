/*using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var services = await _context.ServiceTypes
                .Include(s => s.Gym)
                .ToListAsync();

            return View(services);
        }

        // CREATE GET
        public IActionResult Create()
        {
            return View(new ServiceType
            {
                GymId = 1 // Tek salon olduğu için otomatik
            });
        }

        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Create(ServiceType service)
        {
            if (!ModelState.IsValid)
                return View(service);

            _context.ServiceTypes.Add(service);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _context.ServiceTypes.FindAsync(id);
            if (service == null)
                return NotFound();

            return View(service);
        }

        // EDIT POST
        [HttpPost]
        public async Task<IActionResult> Edit(int id, ServiceType service)
        {
            if (id != service.Id)
                return NotFound();

            if (!ModelState.IsValid)
                return View(service);

            var existing = await _context.ServiceTypes.FindAsync(id);
            if (existing == null)
                return NotFound();

            existing.Name = service.Name;
            existing.Description = service.Description;
            existing.DurationMinutes = service.DurationMinutes;
            existing.Price = service.Price;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.ServiceTypes.FindAsync(id);
            if (service == null)
                return NotFound();

            return View(service);
        }

        // DELETE CONFIRM
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.ServiceTypes.FindAsync(id);

            if (service != null)
            {
                // Eğer randevular tarafından kullanılıyorsa silinemez!
                bool hasAppointments = await _context.Appointments
                    .AnyAsync(a => a.ServiceTypeId == id);

                if (hasAppointments)
                {
                    TempData["Error"] = "Bu hizmet bağlı randevular olduğu için silinemez!";
                    TempData["ErrorSource"] = "ServiceType";
                    return RedirectToAction(nameof(Index));
                }

                _context.ServiceTypes.Remove(service);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}*/


using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ServiceTypeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServiceTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // LIST
        public async Task<IActionResult> Index()
        {
            var services = await _context.ServiceTypes
                .Include(s => s.Gym)
                .ToListAsync();

            return View(services);
        }

        // CREATE GET
        public IActionResult Create()
        {
            ViewBag.Gyms = _context.Gyms
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                })
                .ToList();

            return View();
        }

        // CREATE POST
        [HttpPost]
        public async Task<IActionResult> Create(ServiceType st)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Gyms = _context.Gyms.Select(g => 
                    new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();
                return View(st);
            }

            _context.ServiceTypes.Add(st);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // EDIT GET
        public async Task<IActionResult> Edit(int id)
        {
            var service = await _context.ServiceTypes.FindAsync(id);

            if (service == null)
                return NotFound();

            ViewBag.Gyms = _context.Gyms
                .Select(g => new SelectListItem
                {
                    Value = g.Id.ToString(),
                    Text = g.Name
                })
                .ToList();

            return View(service);
        }

        // EDIT POST
        [HttpPost]
        public async Task<IActionResult> Edit(ServiceType st)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Gyms = _context.Gyms.Select(g =>
                    new SelectListItem { Value = g.Id.ToString(), Text = g.Name }).ToList();
                return View(st);
            }

            _context.ServiceTypes.Update(st);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // DELETE GET
        public async Task<IActionResult> Delete(int id)
        {
            var service = await _context.ServiceTypes
                .Include(s => s.Gym)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
                return NotFound();

            return View(service);
        }

        // DELETE POST
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var service = await _context.ServiceTypes.FindAsync(id);

            if (service == null)
                return NotFound();

            // Eğer trainer hizmetleri bunu kullanıyorsa silme → foreign key hatası olmaması için
            bool isUsed = _context.TrainerServices.Any(ts => ts.ServiceTypeId == id);

            if (isUsed)
            {
                TempData["Error"] = "Bu hizmet bir eğitmen tarafından kullanıldığı için silinemez.";
                return RedirectToAction(nameof(Index));
            }

            _context.ServiceTypes.Remove(service);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
