namespace HRWorkForceSystemBackend.DTOs.SkillgapDTOs
{
    public class EmployeeSkillDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } // Assuming you'll fetch this from the Employee model
        public SkillDto Skill { get; set; }
        public int ProficiencyLevel { get; set; }
        public string Role { get; set; } // Assuming you fetch this from Employee
        public string Department { get; set; } // Assuming you fetch this from Employee
        public DateTime LastUpdated { get; set; }
    }
}