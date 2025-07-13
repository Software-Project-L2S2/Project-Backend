using System.ComponentModel.DataAnnotations;

namespace EmployeeProfileAPI.DTOs.AuthDTOs
{
    public class UpdateProfileDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}