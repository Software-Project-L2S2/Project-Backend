using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeProfileAPI.Models
{
    // This model maps to your existing 'EmployeeProfiles' table.
    public class EmployeeProfile
    {
        [Key]
        [StringLength(50)]
        public string EmployeeID { get; set; } // e.g., "EMP001"

        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public DateTime StartDate { get; set; }
        public int Age { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; }
        public string CompanyLogo { get; set; }
        
        // This is the new property that links this profile to a user.
        public int UserID { get; set; }

        // Your navigation properties remain unchanged.
        public virtual ICollection<ProfileSkill> ProfileSkills { get; set; } = new List<ProfileSkill>();
        public virtual ICollection<ProfileEducation> ProfileEducation { get; set; } = new List<ProfileEducation>();
    }
}