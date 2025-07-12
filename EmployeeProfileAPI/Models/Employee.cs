using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EmployeeProfileAPI.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public DateTime StartDate { get; set; }
        public int Age { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public string ProfileImage { get; set; } // URL to image
        public string CompanyLogo { get; set; }  // URL to company logo

        // Navigation properties for related entities
        public ICollection<Skill> Skills { get; set; } = new List<Skill>(); // Initialize to avoid null reference
        public ICollection<Education> Education { get; set; } = new List<Education>(); // Initialize to avoid null reference
       
        
        // Add other relevant navigation properties if Employee has many WorkforceIssues, LeaveRequests, etc.
        public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; }
    }

}