// D:\Projects\Softwareproject_UOM_2\Project-Backend\HRWorkForceSystemBackend\HRWorkForceSystemBackend\Models\ProjectsModels\Project.cs
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Add this for ICollection

namespace HRWorkForceSystemBackend.Models.ProjectsModels
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string ProjectName { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;

        // NEW: Navigation property to its skill requirements
        public ICollection<ProjectSkillRequirement> ProjectSkillRequirements { get; set; } = new List<ProjectSkillRequirement>();
    }
}