using System.ComponentModel.DataAnnotations;

namespace EmployeeProfileAPI.Models
{
    public class ProjectAssignment
    {
        [Key]
        public int AssignmentID { get; set; }

        [Required]
        public int ProjectID { get; set; }

        [Required]
        public int EmployeeID { get; set; }

        public DateTime AssignedDate { get; set; }

        // Navigation properties should be virtual
        public virtual Project Project { get; set; }
        public virtual Employee Employee { get; set; }
    }
}