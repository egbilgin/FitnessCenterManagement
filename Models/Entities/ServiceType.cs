using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models.Entities
{
    public class ServiceType
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Key to Gym
        [Required]
        public int GymId { get; set; }

        // Service Information
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        // Duration of the service in minutes
        [Required]
        [Range(10, 300)]
        public int DurationMinutes { get; set; }

        // Service price
        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        // Navigation Properties
        public Gym Gym { get; set; }

        public ICollection<TrainerService> TrainerServices { get; set; }
    }
}
