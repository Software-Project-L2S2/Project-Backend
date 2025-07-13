namespace EmployeeProfileAPI.DTOs.UserManagementDTOs
{
    // This DTO includes the 'Id' field needed by the frontend
    public class UserViewDto
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}