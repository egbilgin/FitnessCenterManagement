using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models.Entities
{
    public class TrainerService
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Keys
        [Required]
        public int TrainerId { get; set; }

        [Required]
        public int ServiceTypeId { get; set; }

        // Navigation Properties
        public Trainer Trainer { get; set; }
        public ServiceType ServiceType { get; set; }
    }
}
