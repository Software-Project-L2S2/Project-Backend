using HRWorkForceSystemBackend.Models.AuthModels;

namespace HRWorkForceSystemBackend.Models.PromotionModels
{
    public class Promotion
    {

        public int Id { get; set; }

        public string WorkforceId { get; set; } // FK to Identity User table
        public WorkforceUser WorkforceUser { get; set; }
        public string CurrentRole { get; set; }
        public string RequestedRole { get; set; }
        public string Justification { get; set; }
        public string Achievements { get; set; }
        public string Certifications { get; set; }
        public int DurationInCurrentRole { get; set; } // In months

        public string HRComment { get; set; }
        public string AdminComment { get; set; }

        public string Status { get; set; } // Pending, Reviewed, Approved, Rejected
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
    }
}
