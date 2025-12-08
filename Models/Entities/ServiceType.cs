using FitnessCenterManagement.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models.Entities
{


    public class ServiceType
    {
        public int Id { get; set; }

        [Required]
        public int GymId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        [Required]
        [Range(10, 300)]
        public int DurationMinutes { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal Price { get; set; }

        public Gym Gym { get; set; }
        public ICollection<TrainerService> TrainerServices { get; set; }
    }
}