using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AppointmentApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ✅ 1) Tüm randevuları getir (opsiyonel ama yararlı)
        // GET: /api/AppointmentApi
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Trainer)
                .Include(a => a.ServiceType)
                .Select(a => new
                {
                    a.Id,
                    a.MemberId,
                    a.TrainerId,
                    a.ServiceTypeId,
                    a.StartDateTime,
                    a.EndDateTime,
                    Status = a.Status.ToString(),
                    a.Price
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // ✅ 2) Üye randevularını getir (DOKÜMANIN İSTEDİĞİ FİLTRE!)
        // GET: /api/AppointmentApi/member/1
        [HttpGet("member/{memberId}")]
        public async Task<IActionResult> GetAppointmentsByMember(int memberId)
        {
            // LINQ FILTER: Where(a => a.MemberId == memberId) ✅
            var appointments = await _context.Appointments
                .Where(a => a.MemberId == memberId)
                .Include(a => a.Trainer)
                .Include(a => a.ServiceType)
                .Select(a => new
                {
                    a.Id,
                    MemberId = a.MemberId,
                    Trainer = a.Trainer.FirstName + " " + a.Trainer.LastName,
                    Service = a.ServiceType.Name,
                    a.StartDateTime,
                    a.EndDateTime,
                    Status = a.Status.ToString(),
                    a.Price
                })
                .ToListAsync();

            return Ok(appointments);
        }

        // ✅ 3) Onaylı randevular (ekstra filtre örneği - istersen raporda güç katar)
        // GET: /api/AppointmentApi/approved
        [HttpGet("approved")]
        public async Task<IActionResult> GetApproved()
        {
            var approved = await _context.Appointments
                .Where(a => a.Status == AppointmentStatus.Approved)
                .Select(a => new
                {
                    a.Id,
                    a.MemberId,
                    a.TrainerId,
                    a.StartDateTime,
                    a.EndDateTime,
                    Status = a.Status.ToString(),
                    a.Price
                })
                .ToListAsync();

            return Ok(approved);
        }
    }
}
