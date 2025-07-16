using System.ComponentModel.DataAnnotations;
using System;

namespace HRWorkForceSystemBackend.DTOs.UserManagementDTOs
{
    public class CreateUserDto
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string Role { get; set; }
    }
}