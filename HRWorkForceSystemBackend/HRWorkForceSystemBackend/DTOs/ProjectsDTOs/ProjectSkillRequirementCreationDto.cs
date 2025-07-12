// D:\Projects\Softwareproject_UOM_2\Project-Backend\HRWorkForceSystemBackend\HRWorkForceSystemBackend\DTOs\ProjectsDTOs\ProjectSkillRequirementCreationDto.cs
using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.DTOs.ProjectsDTOs // CORRECTED NAMESPACE
{
    public class ProjectSkillRequirementCreationDto
    {
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public int SkillId { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Required Proficiency Level must be between 1 and 5.")]
        public int RequiredProficiencyLevel { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of resources needed must be at least 1.")]
        public int NumberOfResourcesNeeded { get; set; }
    }
}