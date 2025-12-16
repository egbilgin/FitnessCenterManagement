using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models.Entities;
using FitnessCenterManagement.Models.ViewModels;
using FitnessCenterManagement.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FitnessCenterManagement.Models.Enums;

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            // 1️⃣ Model validation
            if (!ModelState.IsValid)
            {
                await FillDropdownsAsync(model);
                return View(model);
            }

            // 2️⃣ MEMBER rolü → otomatik MemberId
            if (User.IsInRole("Member"))
            {
                var user = await _userManager.GetUserAsync(User);

                var member = await _context.Members
                    .FirstOrDefaultAsync(m => m.UserId == user.Id);

                if (member == null)
                {
                    member = new Member
                    {
                        UserId = user.Id,
                        FirstName = user.UserName,
                        LastName = "Member"
                    };

                    _context.Members.Add(member);
                    await _context.SaveChangesAsync();
                }

                model.MemberId = member.Id;
            }

            // 3️⃣ ADMIN → MemberId doğrulaması
            if (User.IsInRole("Admin"))
            {
                bool memberExists = await _context.Members
                    .AnyAsync(m => m.Id == model.MemberId);

                if (!memberExists)
                {
                    ModelState.AddModelError("", "Geçerli bir üye seçilmedi.");
                    await FillDropdownsAsync(model);
                    return View(model);
                }
            }

            // 4️⃣ SON SAVUNMA
            if (model.MemberId <= 0)
            {
                ModelState.AddModelError("", "Geçerli bir üye bulunamadı.");
                await FillDropdownsAsync(model);
                return View(model);
            }

            // 5️⃣ SERVICE ÇAĞRISI
            var result = await _appointmentService.CreateAppointmentAsync(
                model.MemberId,
                model.TrainerId,
                model.ServiceTypeId,
                model.RequestedStartTime
            );

            switch (result)
            {
                case AppointmentCreateResult.PastDate:
                    ModelState.AddModelError("", "Geçmiş tarihe randevu alınamaz.");
                    break;

                case AppointmentCreateResult.ServiceNotFound:
                    ModelState.AddModelError("", "Seçilen hizmet bulunamadı.");
                    break;

                case AppointmentCreateResult.InvalidDuration:
                    ModelState.AddModelError("", "Hizmet süresi geçersiz.");
                    break;

                case AppointmentCreateResult.TrainerConflict:
                    ModelState.AddModelError("", "Eğitmen bu saat aralığında dolu.");
                    break;

                case AppointmentCreateResult.MemberConflict:
                    ModelState.AddModelError("", "Bu saatlerde başka bir randevunuz var.");
                    break;

                case AppointmentCreateResult.Success:
                    return RedirectToAction(nameof(Index));
            }

            await FillDropdownsAsync(model);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            // Sadece beklemede olan randevu onaylanabilir
            if (appointment.Status != AppointmentStatus.Pending)
                return RedirectToAction(nameof(Index));

            appointment.Status = AppointmentStatus.Approved;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
                return NotFound();

            // Sadece beklemede olan randevu iptal edilebilir
            if (appointment.Status != AppointmentStatus.Pending)
                return RedirectToAction(nameof(Index));

            appointment.Status = AppointmentStatus.Cancelled;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


    }
}
