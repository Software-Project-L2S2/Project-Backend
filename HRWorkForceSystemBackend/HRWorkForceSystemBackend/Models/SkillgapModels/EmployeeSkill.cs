// D:\Projects\Softwareproject_UOM_2\Project-Backend\HRWorkForceSystemBackend\HRWorkForceSystemBackend\Models\SkillgapModels\EmployeeSkill.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRWorkForceSystemBackend.Models.AuthModels; // Assuming HRUser is here
using HRWorkForceSystemBackend.Models.SkillgapModels; // ADDED: To find Skill model

namespace HRWorkForceSystemBackend.Models.SkillgapModels // CORRECTED NAMESPACE
{
    public class EmployeeSkill
    {
        public int Id { get; set; }

        [Required]
        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public HRUser Employee { get; set; }

        [Required]
        public int SkillId { get; set; }
        [ForeignKey("SkillId")]
        public Skill Skill { get; set; } // Skill is found via SkillgapModels using directive

        [Required]
        [Range(1, 5, ErrorMessage = "Proficiency Level must be between 1 and 5.")]
        public int ProficiencyLevel { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}