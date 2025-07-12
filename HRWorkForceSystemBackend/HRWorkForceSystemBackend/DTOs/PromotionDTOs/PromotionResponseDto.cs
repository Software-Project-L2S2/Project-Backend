namespace HRWorkForceSystemBackend.DTOs.PromotionDTOs
{
    public class PromotionResponseDto
    {

        public int Id { get; set; }
        public string WorkforceId { get; set; }
        public string CurrentRole { get; set; }
        public string RequestedRole { get; set; }
        public string Status { get; set; }
        public string HRComment { get; set; }
        public string AdminComment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
