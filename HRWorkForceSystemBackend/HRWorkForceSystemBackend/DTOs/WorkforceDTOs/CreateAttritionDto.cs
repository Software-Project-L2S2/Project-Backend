using System.ComponentModel.DataAnnotations;


namespace HRWorkForceSystemBackend.DTOs.WorkforceDTOs

{
    public class CreateAttritionDto
    {
        [Required]
        public int EmployeeID { get; set; }

        [Required]
        public string Reason { get; set; }
        public string Details { get; set; }

        [Required]
        public DateTime AttritionDate { get; set; }
    }
}