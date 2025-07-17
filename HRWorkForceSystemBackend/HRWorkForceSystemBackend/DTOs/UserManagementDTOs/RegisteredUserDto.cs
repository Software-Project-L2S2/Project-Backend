using System;
using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.DTOs.UserManagementDTOs
{
    public class RegisteredUserDto
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool HasProfile { get; set; }
    }
}
