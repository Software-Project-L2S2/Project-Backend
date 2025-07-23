using System;

namespace HRWorkForceSystemBackend.DTOs.TrainingProgramDTOs
{
    public class CreateTrainingProgramDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TargetSkill { get; set; }
        public int RequiredProficiencyLevel { get; set; }
        public string Mode { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string ResourceLink { get; set; }
        public string TrainerDetails { get; set; }
        public bool AutoAssignment { get; set; }
    }
}