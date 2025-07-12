using System.ComponentModel.DataAnnotations;
namespace EmployeeProfileAPI.Models
{
    public class EmployeeLeave
    {
        [Key]
        public string EmployeeId { get; set; }
        public int TotalLeaves { get; set; } = 15;
        public int LeavesTaken { get; set; } = 0;
        public int AvailableLeaves => TotalLeaves - LeavesTaken;
    }
}