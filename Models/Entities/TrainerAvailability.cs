using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models.Entities
{
    public class TrainerAvailability
    {
        public int Id { get; set; }

        [Required]
        public int TrainerId { get; set; }

        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public Trainer? Trainer { get; set; }
    }

}
