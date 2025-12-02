using FitnessCenterManagement.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using FitnessCenterManagement.Models.ViewModels;

namespace FitnessCenterManagement.Controllers
{
    [Authorize]
    public class AppointmentController : Controller
    {
        private readonly AppointmentService _appointmentService;

        public AppointmentController(AppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // 🔽 Dropdown'ları dolduran yardımcı metot
        private async Task FillDropdownsAsync(AppointmentCreateViewModel model)
        {
            var members = await _appointmentService.GetMembersAsync();
            var trainers = await _appointmentService.GetTrainersAsync();
            var services = await _appointmentService.GetServiceTypesAsync();

            model.Members = members
                .Select(m => new SelectListItem
                {
                    Value = m.Id.ToString(),
                    Text = $"{m.FirstName} {m.LastName}"
                })
                .ToList();

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

        // 🔽 Randevu listesi (Admin + Member görebilir)
        public async Task<IActionResult> Index()
        {
            var appointments = await _appointmentService.GetAppointmentsAsync();
            return View(appointments);
        }

        // 🔽 SADECE MEMBER randevu alabilir (GET)
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Create()
        {
            var model = new AppointmentCreateViewModel
            {
                RequestedStartTime = DateTime.Now.AddHours(1)
            };

            await FillDropdownsAsync(model);
            return View(model);
        }

        // 🔽 SADECE MEMBER randevu alabilir (POST)
        [Authorize(Roles = "Member")]
        [HttpPost]
        public async Task<IActionResult> Create(AppointmentCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await FillDropdownsAsync(model);
                return View(model);
            }

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
