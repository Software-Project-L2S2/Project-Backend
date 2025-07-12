// In folder: DTOs/UserManagementDTOs/UserViewDto.cs

// Make sure this namespace matches EXACTLY in your file.
namespace EmployeeProfileAPI.DTOs.UserManagementDTOs
{
    public class UserViewDto
    {
        public int Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}