

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models.Entities
{
    public class Gym
    {
        // Primary Key
        public int Id { get; set; }

        // Gym Information
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        // Working Hours
        [Required]
        public TimeSpan OpeningTime { get; set; }

        [Required]
        public TimeSpan ClosingTime { get; set; }

        // Navigation Properties
        public ICollection<ServiceType> ServiceTypes { get; set; }
        public ICollection<Trainer> Trainers { get; set; }
    }
}
