using System.ComponentModel.DataAnnotations;
using HRWorkForceSystemBackend.Models.SkillsModels;
using System.Text.Json.Serialization;

namespace HRWorkForceSystemBackend.Models.TrainingProgramModels
{
    public class TrainingAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        public int TrainingProgramId { get; set; }
        public TrainingProgram TrainingProgram { get; set; }

        public string Status { get; set; } = "Assigned"; // e.g., "Assigned", "In Progress", "Completed"
        public string Feedback { get; set; } // Optional feedback from the employee
        public string ProofOfCompletion { get; set; } // Optional link or file path
    }
}