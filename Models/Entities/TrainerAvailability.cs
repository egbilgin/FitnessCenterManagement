using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models.Entities
{
    public class TrainerAvailability
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Key
        [Required]
        public int TrainerId { get; set; }

        // Day of the week (0 = Sunday, 6 = Saturday)
        [Required]
        [Range(0, 6)]
        public int DayOfWeek { get; set; }

        // Availability hours
        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        // Navigation Property
        public Trainer Trainer { get; set; }
    }
}
