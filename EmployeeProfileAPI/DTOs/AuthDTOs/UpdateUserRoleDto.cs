using System.ComponentModel.DataAnnotations;

namespace EmployeeProfileAPI.DTOs.AuthDTOs
{
    public class UpdateUserRoleDto
    {
        [Required]
        public string Role { get; set; }
    }
}