// D:\Projects\Softwareproject_UOM_2\Project-Backend\HRWorkForceSystemBackend\HRWorkForceSystemBackend\Models\SkillgapModels\Skill.cs
using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.Models.SkillgapModels // CORRECTED NAMESPACE
{
    public class Skill
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string SkillName { get; set; }
    }
}