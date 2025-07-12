namespace HRWorkForceSystemBackend.DTOs.PromotionDTOs
{
    public class PromotionRequestDto
    {
        public string RequestedRole { get; set; }
        public string Justification { get; set; }
        public string Achievements { get; set; }
        public string Certifications { get; set; }
        public int DurationInCurrentRole { get; set; }

    }
}
