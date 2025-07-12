// File: DTOs/AnnouncementDTOs.cs
// This should be in a new folder: HRWorkforceSystemBackend/DTOs/

using System.ComponentModel.DataAnnotations;

namespace HRWorkforceSystemBackend.DTOs
{
    // DTO for creating an announcement
    public class AnnouncementCreateDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string TargetRole { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public string SenderRole { get; set; } // "Admin" or "HR"
    }

    // DTO for viewing an announcement, including sender's info
    public class AnnouncementViewDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string TargetRole { get; set; }
        public SenderDto Sender { get; set; }
    }

    // A generic DTO for sender information
    public class SenderDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
    }
}