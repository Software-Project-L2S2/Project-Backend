using System;

namespace EmployeeProfileAPI.DTOs
{
    public class UserProfileResponse
    {
        public string EmployeeID { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string SkillLevel { get; set; }
        public DateTime? StartDate { get; set; }
        public int ProjectsCompleted { get; set; }
        public string ProfileImage { get; set; }
    }
}
