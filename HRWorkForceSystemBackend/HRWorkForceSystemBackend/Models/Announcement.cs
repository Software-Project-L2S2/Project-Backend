
using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; }
        
        [Required]
        [StringLength(50)]
        public string  Audience { get; set; } // "All", "HRUsers", "Admins", "WorkforceUsers"
        
        [StringLength(100)]
        public string TagRole { get; set; }
        
        [Required]
        [StringLength(1000)]
        public string Note { get; set; }
        
        [Required]
        [StringLength(20)]
        public string CommunicationType { get; set; } // "email" or "sms"
        
        public DateTime CreatedAt { get; set; }
        
        public int? EmailsSent { get; set; } // Track how many emails were sent
        
        public bool IsEmailSent { get; set; } = false; // Track if emails were sent successfully
    }
    }