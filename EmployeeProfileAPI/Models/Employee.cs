using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // <-- 1. MAKE SURE THIS LINE IS HERE

namespace EmployeeProfileAPI.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // <-- 2. THIS LINE IS THE FIX
        public int EmployeeID { get; set; }

        // ... all other properties remain the same
        [Required]
        public string Name { get; set; }

        [Required]
        public string Designation { get; set; }
        
        [Required]
        public string Department { get; set; }

        [Required]
        public string Gender { get; set; }

        public DateTime StartDate { get; set; }
        public int Age { get; set; }

        [Required]
        public string Contact { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string? ProfileImage { get; set; }
        public string? CompanyLogo { get; set; }

        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<Education> Education { get; set; } = new List<Education>();
        public ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
    }
}