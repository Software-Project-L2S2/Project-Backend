using System.Collections.Generic;

namespace HRWorkForceSystemBackend.DTOs.TrainingProgramDTOs
{
    public class ManualAssignmentDto
    {
        public int TrainingProgramId { get; set; }
        public List<int> EmployeeIds { get; set; }
    }
}