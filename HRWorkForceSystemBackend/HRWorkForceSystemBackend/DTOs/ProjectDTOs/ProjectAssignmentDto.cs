using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.DTOs.ProjectDTOs
{
    public class ProjectAssignmentDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be a positive number.")]
        public int ProjectID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeID must be a positive number.")]
        public int EmployeeID { get; set; }
    }
}