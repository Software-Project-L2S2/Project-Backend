using System.ComponentModel.DataAnnotations;


namespace HRWorkForceSystemBackend.DTOs.UserProfileDTOs
{
    public class UpdateEmployeeDto
    {
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public DateTime StartDate { get; set; }
        public int Age { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
    }
}