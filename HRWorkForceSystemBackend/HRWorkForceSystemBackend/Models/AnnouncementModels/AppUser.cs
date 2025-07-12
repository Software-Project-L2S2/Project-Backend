// File: Models/AppUser.cs

using System.ComponentModel.DataAnnotations;

namespace HRWorkforceSystemBackend.Models.AnnouncementModels
{
    public class AppUser
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Role { get; set; } // Can be "Admin", "HR", or "Workforce"
    }
}