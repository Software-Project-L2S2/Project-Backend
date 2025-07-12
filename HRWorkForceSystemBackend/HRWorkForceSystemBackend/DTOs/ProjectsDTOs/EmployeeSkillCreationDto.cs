// HRWorkForceSystemBackend\DTOs\EmployeeSkillCreationDto.cs
// D:\Projects\Softwareproject_UOM_2\Project-Backend\HRWorkForceSystemBackend\HRWorkForceSystemBackend\DTOs\ProjectsDTOs\EmployeeSkillCreationDto.cs
using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.DTOs.ProjectsDTOs // CORRECTED NAMESPACE
{
    public class EmployeeSkillCreationDto
    {
        [Required]
        public int EmployeeId { get; set; }
        [Required]
        public int SkillId { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Proficiency Level must be between 1 and 5.")]
        public int ProficiencyLevel { get; set; }
    }
}