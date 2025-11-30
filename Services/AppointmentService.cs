using Microsoft.EntityFrameworkCore;
using FitnessCenterManagement.Data;
using FitnessCenterManagement.Models.Entities;
using FitnessCenterManagement.Models.Enums;

namespace FitnessCenterManagement.Services
{
    public class AppointmentService
    {
        private readonly ApplicationDbContext _context;

        public AppointmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Trainer)
                .Include(a => a.ServiceType)
                .OrderBy(a => a.StartDateTime)
                .ToListAsync();
        }

        public async Task<List<Member>> GetMembersAsync()
        {
            return await _context.Members
                .OrderBy(m => m.FirstName)
                .ToListAsync();
        }

        public async Task<List<Trainer>> GetTrainersAsync()
        {
            return await _context.Trainers
                .OrderBy(t => t.FirstName)
                .ToListAsync();
        }

        public async Task<List<ServiceType>> GetServiceTypesAsync()
        {
            return await _context.ServiceTypes
                .OrderBy(s => s.Name)
                .ToListAsync();
        }



        public async Task<bool> CreateAppointmentAsync(
            int memberId,
            int trainerId,
            int serviceTypeId,
            DateTime requestedStartTime)
        {
            //geçmiş tartnen mi alacak randevyu onu engelliyoruz
            if (requestedStartTime < DateTime.Now)
            {
                return false;
            }


            // 1) Hizmeti bul
            var serviceType = await _context.ServiceTypes
                .FirstOrDefaultAsync(s => s.Id == serviceTypeId);

            if (serviceType == null)
            {
                // Böyle bir hizmet yok → randevu oluşturma
                return false;
            }

            // 2.1️ ServiceType süre kontrolü
            if (serviceType.DurationMinutes <= 0)
            {
                return false;
            }


            // 2) Bitiş saatini hesapla
            DateTime calculatedEndTime =
                requestedStartTime.AddMinutes(serviceType.DurationMinutes);



            // 3) Trainer o gün / saatte çalışıyor mu? (availability)
            /*bool isTrainerAvailable = await _context.TrainerAvailabilities.AnyAsync(a =>
                a.TrainerId == trainerId &&
                a.DayOfWeek == (int)requestedStartTime.DayOfWeek &&
                a.StartTime <= requestedStartTime.TimeOfDay &&
                a.EndTime >= calculatedEndTime.TimeOfDay
            );

            if (!isTrainerAvailable)
            {
                // Trainer bu saat aralığında çalışmıyor
                return false;
            }*/

            // 4) Aynı trainer için çakışan randevu var mı?
            bool hasConflict = await _context.Appointments.AnyAsync(a =>
                a.TrainerId == trainerId &&
                a.Status != AppointmentStatus.Cancelled &&
                requestedStartTime < a.EndDateTime &&
                calculatedEndTime > a.StartDateTime
            );

            if (hasConflict)
            {
                // Bu saat aralığı zaten dolu
                return false;
            }

            bool memberHasConflict = await _context.Appointments.AnyAsync(a =>
    a.MemberId == memberId &&
    a.Status != AppointmentStatus.Cancelled &&
    requestedStartTime < a.EndDateTime &&
    calculatedEndTime > a.StartDateTime
);

            if (memberHasConflict)
            {
                return false;
            }


            // 5) Artık güvenli → randevuyu oluştur
            var appointment = new Appointment
            {
                MemberId = memberId,
                TrainerId = trainerId,
                ServiceTypeId = serviceTypeId,
                StartDateTime = requestedStartTime,
                EndDateTime = calculatedEndTime,
                Status = AppointmentStatus.Pending,
                Price = serviceType.Price,
                Notes = string.Empty
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // 6) Başarılı
            return true;
        }
    }
}
