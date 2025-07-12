// File: Models/Announcement.cs
// This file should be placed in HRWorkForceSystemBackend/Models/

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRWorkForceSystemBackend.Models.AuthModels; // Assuming Admin and HRUser are here

namespace HRWorkForceSystemBackend.Models
{
    public class Announcement
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string TargetRole { get; set; } // e.g., "All", "Workforce", "HR"

        // --- MODIFIED SECTION ---
        // Nullable foreign keys for each possible sender type
        public int? AdminSenderId { get; set; }
        public int? HRUserSenderId { get; set; }

        // Navigation properties to the sender
        [ForeignKey("AdminSenderId")]
        public virtual Admin AdminSender { get; set; }

        [ForeignKey("HRUserSenderId")]
        public virtual HRUser HRUserSender { get; set; }
    }
}