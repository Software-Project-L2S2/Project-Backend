using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRWorkForceSystemBackend.Models.ProjectModels
{
    public class Project
    {
        [Key]
        public int ProjectID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }  // Matches SQL column 'Name'

        [StringLength(50)]
        public string Status { get; set; } // Pending, In Progress, Completed

        [StringLength(500)]
        public string Skills { get; set; } // Could be comma-separated skills

        public int EmployeeCount { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        // Navigation property (optional)
        public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; }

        
    }
}