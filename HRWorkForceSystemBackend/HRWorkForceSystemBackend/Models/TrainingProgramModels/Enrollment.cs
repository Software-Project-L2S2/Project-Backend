
using System.Text.Json.Serialization;




namespace HRWorkForceSystemBackend.Models.TrainingProgramModels
{
    public class Enrollment
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int TrainingProgramId { get; set; }

        public TrainingProgram TrainingProgram { get; set; }
    }
}
