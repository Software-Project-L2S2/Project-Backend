#nullable enable
using System;
using System.Collections.Generic;
using HRWorkForceSystemBackend.Models.SkillsModels;
using System.Text.Json.Serialization;

namespace HRWorkForceSystemBackend.Models.TrainingProgramModels
{
    public class TrainingProgram
    {
        public int Id { get; set; }

        // --- THE FIX IS HERE ---
        // By adding 'required', you enforce that Name and Description must be provided
        // when a new TrainingProgram is created.
        public required string Name { get; set; }
        public required string Description { get; set; }

        // --- Fields for HR/Admin Assigned Training ---
        public string? TargetSkill { get; set; }
        public int RequiredProficiencyLevel { get; set; }
        public string? Mode { get; set; } // "Online" or "Offline"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ResourceLink { get; set; }
        public string? TrainerDetails { get; set; }
        public bool AutoAssignment { get; set; } = false;

        // --- Field for Workforce Self-Enrollment ---
        public int Availability { get; set; }

        // Relationships (These are already correct because they are initialized)
        public ICollection<TrainingAssignment> Assignments { get; set; } = new List<TrainingAssignment>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}