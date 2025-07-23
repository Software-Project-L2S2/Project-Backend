#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; 
using HRWorkForceSystemBackend.Models.ProjectModels;
// <-- 1. MAKE SURE THIS LINE IS HERE
using System.Text.Json.Serialization;

namespace HRWorkForceSystemBackend.Models.SkillsModels
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // <-- 2. THIS LINE IS THE FIX
        public int EmployeeID { get; set; }

        // ... all other properties remain the same
        [Required]
        public required string Name { get; set; }

        [Required]
        public required string Designation { get; set; }

        [Required]
        public required string Department { get; set; }

        [Required]
        public required string Gender { get; set; }

        public required DateTime StartDate { get; set; }
        public int Age { get; set; }

        [Required]
        public required string Contact { get; set; }

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public string? ProfileImage { get; set; }
        public string? CompanyLogo { get; set; }

        public ICollection<Skill> Skills { get; set; } = new List<Skill>();
        public ICollection<Education> Education { get; set; } = new List<Education>();
        public ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();
    }
}