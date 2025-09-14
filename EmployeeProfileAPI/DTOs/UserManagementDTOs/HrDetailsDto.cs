namespace EmployeeProfileAPI.DTOs.UserManagementDTOs
{
    public class HrDetailsDto
    {
        public string Email { get; set; }  // Used to identify the HRUser
        public string HRId { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public DateTime StartDate { get; set; }
        public string Contact { get; set; }
        public string Accounts { get; set; }

    }
}
