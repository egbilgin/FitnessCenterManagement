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

        // ----------------------------------------------------
        // 1️⃣ RANDEVU LİSTESİ
        // ----------------------------------------------------
        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            return await _context.Appointments
                .Include(a => a.Member)
                .Include(a => a.Trainer)
                .Include(a => a.ServiceType)
                .OrderBy(a => a.StartDateTime)
                .ToListAsync();
        }

        // ----------------------------------------------------
        // 2️⃣ DROPDOWN VERİLERİ
        // ----------------------------------------------------
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

        // ----------------------------------------------------
        // 3️⃣ RANDEVU OLUŞTURMA (ÇAKIŞMA KONTROLLÜ)
        // ----------------------------------------------------
        public async Task<AppointmentCreateResult> CreateAppointmentAsync(
            int memberId,
            int trainerId,
            int serviceTypeId,
            DateTime requestedStartTime)
        {
            // 3.1 Geçmiş tarih kontrolü (UTC)
            if (requestedStartTime < DateTime.UtcNow)
            {
                return AppointmentCreateResult.PastDate;
            }

            // 3.2 Hizmeti bul
            var serviceType = await _context.ServiceTypes
                .FirstOrDefaultAsync(s => s.Id == serviceTypeId);

            if (serviceType == null)
            {
                return AppointmentCreateResult.ServiceNotFound;
            }

            // 3.3 Süre kontrolü
            if (serviceType.DurationMinutes <= 0)
            {
                return AppointmentCreateResult.InvalidDuration;
            }

            // 3.4 Bitiş zamanını hesapla
            DateTime calculatedEndTime =
                requestedStartTime.AddMinutes(serviceType.DurationMinutes);

            // ------------------------------------------------
            // 3.5 TRAINER ÇAKIŞMA KONTROLÜ
            // ------------------------------------------------
            bool trainerHasConflict = await _context.Appointments.AnyAsync(a =>
                a.TrainerId == trainerId &&
                a.Status != AppointmentStatus.Cancelled &&
                requestedStartTime < a.EndDateTime &&
                calculatedEndTime > a.StartDateTime
            );

            if (trainerHasConflict)
            {
                return AppointmentCreateResult.TrainerConflict;
            }

            // ------------------------------------------------
            // 3.6 MEMBER ÇAKIŞMA KONTROLÜ
            // ------------------------------------------------
            bool memberHasConflict = await _context.Appointments.AnyAsync(a =>
                a.MemberId == memberId &&
                a.Status != AppointmentStatus.Cancelled &&
                requestedStartTime < a.EndDateTime &&
                calculatedEndTime > a.StartDateTime
            );

            if (memberHasConflict)
            {
                return AppointmentCreateResult.MemberConflict;
            }

            // ------------------------------------------------
            // 3.7 RANDEVUYU OLUŞTUR
            // ------------------------------------------------
            var appointment = new Appointment
            {
                MemberId = memberId,
                TrainerId = trainerId,
                ServiceTypeId = serviceTypeId,
                StartDateTime = requestedStartTime,
                EndDateTime = calculatedEndTime,
                Status = AppointmentStatus.Pending,
                Price = serviceType.Price,
                Notes = null
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return AppointmentCreateResult.Success;
        }
    }

    // ----------------------------------------------------
    // 4️⃣ RANDEVU OLUŞTURMA SONUÇLARI
    // ----------------------------------------------------
    public enum AppointmentCreateResult
    {
        Success,
        PastDate,
        ServiceNotFound,
        InvalidDuration,
        TrainerConflict,
        MemberConflict
    }
}
