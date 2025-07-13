using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using EmployeeProfileAPI.Models.AuthModels; // IMPORTANT: Add this using statement for the User model

namespace EmployeeProfileAPI.Models
{
    public class EmployeeProfile
    {
        [Key]
        [StringLength(50)]
        public string EmployeeID { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Designation { get; set; }

        [Required]
        [StringLength(100)]
        public string Department { get; set; }

        [Required]
        [StringLength(50)]
        public string Gender { get; set; }

        [Required]
        public System.DateTime StartDate { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        [StringLength(50)]
        public string Contact { get; set; }

        [Required]
        [StringLength(255)]
        public string Email { get; set; }

        public string? ProfileImage { get; set; }
        public string? CompanyLogo { get; set; }

        // Foreign Key to Users table
        public int UserID { get; set; }

        // === THIS IS THE CRITICAL ADDITION ===
        // The navigation property that links UserID to a User object.
        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        // Navigation Properties for Skills and Education
        public virtual ICollection<ProfileSkill> ProfileSkills { get; set; } = new List<ProfileSkill>();
        public virtual ICollection<ProfileEducation> ProfileEducation { get; set; } = new List<ProfileEducation>();
    }
}