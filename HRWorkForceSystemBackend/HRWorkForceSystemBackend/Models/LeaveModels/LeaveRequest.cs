using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.Models.LeaveModels
{
    public class LeaveRequest
    {
        public int Id { get; set; }

        [Required]
        public string LeaveType { get; set; } = string.Empty;

        [Required]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public string EmployeeName { get; set; } = string.Empty;

        [Required]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int Duration => (EndDate - StartDate).Days + 1;

        public string? DocumentPath { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
