namespace HRWorkForceSystemBackend.Models.FeedbackModels
{
    public class Feedback
    {
        public int Id { get; set; }
        public string Emoji { get; set; }
        public string Text { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
    }
}
