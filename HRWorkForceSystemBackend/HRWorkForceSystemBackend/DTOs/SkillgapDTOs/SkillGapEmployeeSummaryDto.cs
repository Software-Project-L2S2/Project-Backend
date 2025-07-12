// HRWorkForceSystemBackend\DTOs\SkillgapDTOs\SkillGapEmployeeSummaryDto.cs
using System;
using System.Collections.Generic;

namespace HRWorkForceSystemBackend.DTOs.SkillgapDTOs // CORRECTED NAMESPACE
{
    public class SkillGapEmployeeSummaryDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public List<string> SkillsWithProficiency { get; set; } = new List<string>();
        public DateTime LastUpdated { get; set; }
    }
}