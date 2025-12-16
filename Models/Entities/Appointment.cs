using System;
using System.ComponentModel.DataAnnotations;
using FitnessCenterManagement.Models.Enums;

namespace FitnessCenterManagement.Models.Entities
{
    public class Appointment
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Keys
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int TrainerId { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }

        // Appointment Time
        [Required]
        public DateTime StartDateTime { get; set; }

        [Required]
        public DateTime EndDateTime { get; set; }

        // Appointment Status
        [Required]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        // Price at the time of booking
        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        [StringLength(250)]
        public string? Notes { get; set; }

        // Navigation Properties
        public Member Member { get; set; }
        public Trainer Trainer { get; set; }
        public ServiceType ServiceType { get; set; }


    }
}
