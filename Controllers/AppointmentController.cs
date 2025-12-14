using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models.Entities;
using FitnessCenterManagement.Models.ViewModels;
using FitnessCenterManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
//14 aralık
namespace FitnessCenterManagement.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly AppointmentService _appointmentService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AppointmentController(
            AppointmentService appointmentService,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _appointmentService = appointmentService;
            _userManager = userManager;
            _context = context;
        }

        // ---------------------------------------
        // DROPDOWN (SADECE ADMIN İÇİN)
        // ---------------------------------------
        private async Task FillDropdownsAsync(AppointmentCreateViewModel model)
        {
            var trainers = await _appointmentService.GetTrainersAsync();
            var services = await _appointmentService.GetServiceTypesAsync();

            // Admin ise members dropdown'ı da gelir
            if (User.IsInRole("Admin"))
            {
                var members = await _appointmentService.GetMembersAsync();

                model.Members = members
                    .Select(m => new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = $"{m.FirstName} {m.LastName}"
                    })
                    .ToList();
            }

            model.Trainers = trainers
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = $"{t.FirstName} {t.LastName}"
                })
                .ToList();

            model.ServiceTypes = services
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToList();
        }

        // ---------------------------------------
        // INDEX — Admin tümünü, Member kendi randevusunu görür
        // ---------------------------------------
        public async Task<IActionResult> Index()
        {
            var allAppointments = await _appointmentService.GetAppointmentsAsync();

            // ADMIN → tüm randevularıを見る
            if (User.IsInRole("Admin"))
                return View(allAppointments);

            // MEMBER → sadece kendi randevularını görür
            var user = await _userManager.GetUserAsync(User);
            var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == user.Id);

            if (member == null)
                return View(new List<Appointment>());

            var myAppointments = allAppointments.Where(a => a.MemberId == member.Id).ToList();

            return View(myAppointments);
        }

        // ---------------------------------------
        // CREATE — Randevu formu
        // ---------------------------------------
        public async Task<IActionResult> Create()
        {
            var model = new AppointmentCreateViewModel
            {
                RequestedStartTime = DateTime.Now.AddHours(1)
    .AddSeconds(-DateTime.Now.Second)
    .AddMilliseconds(-DateTime.Now.Millisecond)

            };

            await FillDropdownsAsync(model);
            return View(model);
        }

        // ---------------------------------------
        // CREATE — Randevu kayıt
        // ---------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await FillDropdownsAsync(model);
                return View(model);
            }

            // MEMBER → MemberId DB’den otomatik bulunur
            if (User.IsInRole("Member"))
            {
                var user = await _userManager.GetUserAsync(User);
                var member = await _context.Members.FirstOrDefaultAsync(m => m.UserId == user.Id);

                if (member == null)
                {
                    ModelState.AddModelError("", "Üye kaydı bulunamadı.");
                    return View(model);
                }

                model.MemberId = member.Id;  // 🔥 OTOMATİK MEMBER ID
            }

            // ADMIN → dropdown’dan gelen MemberId zaten hazır

            bool success = await _appointmentService.CreateAppointmentAsync(
                model.MemberId,
                model.TrainerId,
                model.ServiceTypeId,
                model.RequestedStartTime
            );

            if (!success)
            {
                ModelState.AddModelError("", "Randevu oluşturulamadı.");
                await FillDropdownsAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
