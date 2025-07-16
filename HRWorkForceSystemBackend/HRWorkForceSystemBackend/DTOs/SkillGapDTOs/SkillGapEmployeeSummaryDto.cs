using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace HRWorkForceSystemBackend.DTOs.SkillgapDTOs
{
    public class SkillGapEmployeeSummaryDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string Designation { get; set; }
        public List<string> Skills { get; set; } = new List<string>();
        public List<string> AssignedProjects { get; set; } = new List<string>();
    }
}