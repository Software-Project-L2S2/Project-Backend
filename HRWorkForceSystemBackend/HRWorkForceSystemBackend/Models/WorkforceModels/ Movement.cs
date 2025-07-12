namespace HRWorkForceSystemBackend.Models.WorkforceModels
{
    public class Movement
    {
        public int Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }
}