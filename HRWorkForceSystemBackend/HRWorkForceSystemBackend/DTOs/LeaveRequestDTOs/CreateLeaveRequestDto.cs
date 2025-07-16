using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.DTOs.LeaveRequestDTOs
{
    public class CreateLeaveRequestDto
    {
        [Required] public string LeaveType { get; set; }
        [Required] public string Reason { get; set; }
        [Required] public string EmployeeName { get; set; }
        [Required] public string EmployeeId { get; set; }
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime EndDate { get; set; }
        public IFormFile? Document { get; set; }
    }
}
