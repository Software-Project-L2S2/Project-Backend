namespace HRWorkForceSystemBackend.DTOs.UserProfileDTOs
{
    public class UpdateMovementDto
    {
        public int Id { get; set; }
        public int EmployeeID { get; set; }
        public string NewPosition { get; set; }
        public string NewDepartment { get; set; }
        public string Description { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}