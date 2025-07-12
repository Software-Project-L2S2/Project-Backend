// D:\Projects\Softwareproject_UOM_2\Project-Backend\HRWorkForceSystemBackend\HRWorkForceSystemBackend\Models\ProjectsModels\ProjectSkillRequirement.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRWorkForceSystemBackend.Models.SkillgapModels; // ADDED: To find Skill model

namespace HRWorkForceSystemBackend.Models.ProjectsModels // CORRECTED NAMESPACE
{
    public class ProjectSkillRequirement
    {
        public int Id { get; set; }

        [Required]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public Project Project { get; set; } // Project is in the same namespace now

        [Required]
        public int SkillId { get; set; }
        [ForeignKey("SkillId")]
        public Skill Skill { get; set; } // Skill is found via SkillgapModels using directive

        [Required]
        [Range(1, 5, ErrorMessage = "Required Proficiency Level must be between 1 and 5.")]
        public int RequiredProficiencyLevel { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of resources needed must be at least 1.")]
        public int NumberOfResourcesNeeded { get; set; }
    }
}