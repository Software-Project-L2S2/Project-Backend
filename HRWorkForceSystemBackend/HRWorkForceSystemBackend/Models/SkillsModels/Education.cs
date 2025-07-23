using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HRWorkForceSystemBackend.Models.SkillsModels
{
    public class Education
    {
        [Key]

        public int EducationID { get; set; }
        public int EmployeeID { get; set; }  // Should match frontend
        public string Qualification { get; set; }


        // Foreign Key to Employee


        // Navigation property
        [ForeignKey("EmployeeID")]
        public Employee Employee { get; set; }
    }
}