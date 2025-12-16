using FitnessCenterManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitnessCenterManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TrainerApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TrainerApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /api/TrainerApi
        [HttpGet]
        public async Task<IActionResult> GetAllTrainers()
        {
            var trainers = await _context.Trainers
                .OrderBy(t => t.FirstName)
                .Select(t => new
                {
                    t.Id,
                    Name = t.FirstName + " " + t.LastName
                })
                .ToListAsync();

            return Ok(trainers);
        }
    }
}
