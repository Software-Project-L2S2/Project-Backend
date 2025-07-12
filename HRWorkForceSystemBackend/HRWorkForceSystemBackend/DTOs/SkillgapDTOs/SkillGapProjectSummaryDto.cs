// HRWorkForceSystemBackend\DTOs\SkillgapDTOs\SkillGapProjectSummaryDto.cs
using System.Collections.Generic;
// No need for HRWorkForceSystemBackend.Models.WorkforceModels here if SkillDto is in the same SkillgapDTOs namespace

namespace HRWorkForceSystemBackend.DTOs.SkillgapDTOs // CORRECTED NAMESPACE
{
    public class SkillGapProjectSummaryDto
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<SkillDto> RequiredSkills { get; set; } = new List<SkillDto>();
        public List<string> SkillsNeeded { get; set; } = new List<string>();
        public int AvailableEmployees { get; set; }
        public string Status { get; set; }
    }
}