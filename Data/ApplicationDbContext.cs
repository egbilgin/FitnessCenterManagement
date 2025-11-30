using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using FitnessCenterManagement.Models.Entities;

namespace FitnessCenterManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Gym> Gyms { get; set; }
        public DbSet<ServiceType> ServiceTypes { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainerService> TrainerServices { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AiRecommendationRequest> AiRecommendationRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ServiceType>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            builder.Entity<Appointment>()
                .Property(a => a.Price)
                .HasPrecision(10, 2);

            builder.Entity<Appointment>()
    .HasOne(a => a.Trainer)
    .WithMany()
    .HasForeignKey(a => a.TrainerId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.ServiceType)
                .WithMany()
                .HasForeignKey(a => a.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TrainerService>()
    .HasOne(ts => ts.Trainer)
    .WithMany()
    .HasForeignKey(ts => ts.TrainerId)
    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<TrainerService>()
                .HasOne(ts => ts.ServiceType)
                .WithMany()
                .HasForeignKey(ts => ts.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);


        }



    }
}
