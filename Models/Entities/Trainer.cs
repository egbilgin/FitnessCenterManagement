using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models.Entities
{
    public class Trainer
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Key to Gym
        [Required]
        public int GymId { get; set; }

        // Trainer Information
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(150)]
        public string Specializations { get; set; }

        // Active / Passive status
        public bool IsActive { get; set; } = true;

        // Navigation Properties
        public Gym Gym { get; set; }

        public ICollection<TrainerService> TrainerServices { get; set; }
        public ICollection<TrainerAvailability> Availabilities { get; set; }
    }
}
