namespace EmployeeProfileAPI.DTOs.UserDTOs
{
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
}