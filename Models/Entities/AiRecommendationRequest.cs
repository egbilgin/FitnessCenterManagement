using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterManagement.Models.Entities
{
    public class AiRecommendationRequest
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Key to Member
        [Required]
        public int MemberId { get; set; }

        // Input data sent to AI
        [Range(100, 250)]
        public int? HeightCm { get; set; }

        [Range(30, 300)]
        public int? WeightKg { get; set; }

        [StringLength(100)]
        public string ActivityLevel { get; set; }

        [StringLength(250)]
        public string GoalDescription { get; set; }

        // AI Response
        [Required]
        public string AiResponse { get; set; }

        // Request timestamp
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Property
        public Member Member { get; set; }
    }
}
