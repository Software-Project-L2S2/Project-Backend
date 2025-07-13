namespace EmployeeProfileAPI.DTOs
{
    public class ProfileSkillsDto
    {
        public int EmployeeID { get; set; }  // CHANGED from string to int
        public string SkillName { get; set; }
        public string Description { get; set; }
        public string Level { get; set; }
    }
}
