// In DTOs/CreateMovementDto.cs
using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.DTOs.WorkforceDTOs
{
    public class CreateMovementDto
    {
        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public string MovementType { get; set; }

        public string NewPosition { get; set; }
        public string NewDepartment { get; set; }
        public string Description { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }
    }
}