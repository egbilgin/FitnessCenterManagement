using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models.Entities
{
    public class Member
    {
        // Primary Key
        public int Id { get; set; }

        // Identity User ID (ASP.NET Identity)
        [Required]
        public string UserId { get; set; }

        // Member Profile Information
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Range(100, 250)]
        public int? HeightCm { get; set; }

        [Range(30, 300)]
        public int? WeightKg { get; set; }

        [StringLength(250)]
        public string? Goal { get; set; }

        // Navigation Properties
        public ICollection<Appointment> Appointments { get; set; }
        public ICollection<AiRecommendationRequest> AiRecommendationRequests { get; set; }
    }
}
