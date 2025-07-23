using System.Collections.Generic;


namespace HRWorkForceSystemBackend.DTOs.TrainingProgramDTOs
{
    public class MyProgramDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TargetSkill { get; set; }
        public string Status { get; set; } // "Assigned", "In Progress", "Completed"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Mode { get; set; }
        public string AssignmentType { get; set; } // "Mandatory" or "Enrolled"
    }
}