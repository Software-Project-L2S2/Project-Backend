using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRWorkForceSystemBackend.Models.SkillsModels
{
    public class Skill
    {
        [Key]
        public int SkillID { get; set; }
        public string SkillName { get; set; }
        public string Description { get; set; }
        public int Level { get; set; } // e.g., 1-5

        // Foreign Key to Employee
        public int EmployeeID { get; set; }

        // Navigation property
        [ForeignKey("EmployeeID")]
        public Employee Employee { get; set; }
    }
}