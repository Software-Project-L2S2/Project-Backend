namespace HRWorkForceSystemBackend.DTOs.TrainingProgramDTOs
{
    public class TrainingProgramDto
    {

        public int? Id { get; set; } // Nullable for update
        public string Name { get; set; }
        public string Description { get; set; }
        public int Availability { get; set; }
    }
}
