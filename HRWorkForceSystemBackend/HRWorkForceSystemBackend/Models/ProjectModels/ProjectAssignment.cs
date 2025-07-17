using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRWorkForceSystemBackend.Models.SkillsModels; // To find the Employee class

namespace HRWorkForceSystemBackend.Models.ProjectModels
{
    public class ProjectAssignment
    {
        [Key]
        public int AssignmentID { get; set; }

        [Required]
        public int ProjectID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        public DateTime AssignedDate { get; set; }

        // Navigation properties for Entity Framework Core
        [ForeignKey("ProjectID")]
        public virtual Project Project { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; }
    }
}