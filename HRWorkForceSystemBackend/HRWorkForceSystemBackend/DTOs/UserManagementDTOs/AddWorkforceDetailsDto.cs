// DTOs/UserManagementDTOs/AddWorkforceDetailsDto.cs
namespace HRWorkForceSystemBackend.DTOs.UserManagementDTOs
{
    public class AddWorkforceDetailsDto
    {
        public string Email { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string CurrentRole { get; set; } = string.Empty;
    }
}