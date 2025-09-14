using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeProfileAPI.Models
{
    public class PasswordChangeVerification
    {
        [Key]
        public int VerificationId { get; set; } // The Primary Key for this table

        [Required]
        public int UserID { get; set; } // Foreign key to the UserProfile table

        [Required]
        public string VerificationToken { get; set; }

        [Required]
        public string NewPasswordHash { get; set; }

        [Required]
        public DateTime TokenExpiry { get; set; }

        [Required]
        public bool IsUsed { get; set; }
    }
}