using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeProfileAPI.Data
{
    public class UserProfile
    {
        [Key]
        [Column("EmployeeID")]
        public string EmployeeID { get; set; }

        [Column("FullName")]
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Column("Department")]
        [Required]
        [StringLength(50)]
        public string Department { get; set; }

        [Column("Email")]
        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Column("SkillLevel")]
        [StringLength(20)]
        public string SkillLevel { get; set; }

        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        [Column("ProjectsCompleted")]
        public int ProjectsCompleted { get; set; }

        [Column("ProfileImage")]
        [StringLength(255)]
        public string ProfileImage { get; set; }

        [Column("PasswordHash")]
        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; }

        // Add other properties as needed
       
    }
}