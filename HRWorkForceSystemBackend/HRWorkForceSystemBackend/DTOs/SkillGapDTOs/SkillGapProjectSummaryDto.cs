using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.DTOs.SkillgapDTOs
{
    public class SkillGapProjectSummaryDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<string> RequiredSkills { get; set; } = new List<string>();
        public int EmployeesNeeded { get; set; }
        public int EmployeesAssigned { get; set; }
        public int EmployeesAvailable { get; set; }
        public string Status { get; set; } // e.g., "Good", "Low", "Critical"
        public List<string> SkillDeficits { get; set; } = new List<string>();
    }
}