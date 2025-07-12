namespace HRWorkForceSystemBackend.Models.WorkforceModels
{
	public class Attrition
	{
		public int Id { get; set; }
		public string EmployeeId { get; set; } = string.Empty;
		public string Name { get; set; } = string.Empty;
		public string Department { get; set; } = string.Empty;
		public string Position { get; set; } = string.Empty;
		public DateTime ExitDate { get; set; } = DateTime.UtcNow;
		public string Notes { get; set; } = string.Empty;
	}
}