using System.ComponentModel.DataAnnotations;
using System;

namespace HRWorkForceSystemBackend.DTOs.UserManagementDTOs
{
    public class AddEmployeeDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Designation { get; set; }
        [Required]
        public string Department { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string Contact { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
