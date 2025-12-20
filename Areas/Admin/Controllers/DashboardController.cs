using System;
using System.Linq;
using FitnessCenterManagement.Areas.Admin.ViewModels;
using FitnessCenterManagement.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FitnessCenterManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var model = new DashboardViewModel
            {
                // Identity users (tüm kullanıcılar)
                TotalMembers = _context.Users.Count(),

                // Tüm randevular
                TotalAppointments = _context.Appointments.Count(),

                // Bugün başlayan randevular
                TodayAppointments = _context.Appointments
                    .Count(a =>
                        a.StartDateTime >= today &&
                        a.StartDateTime < tomorrow),

                // Aktif trainer sayısı
                ActiveTrainers = _context.Trainers.Count()
            };

            return View(model);
        }
    }
}
