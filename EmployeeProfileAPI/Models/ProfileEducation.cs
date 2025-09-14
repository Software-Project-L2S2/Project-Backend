using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EmployeeProfileAPI.Models
{
    public class ProfileEducation
    {
        [Key]
        public int EducationID { get; set; }

        [Required]
        public string Qualification { get; set; }

        // Foreign Key to EmployeeProfile
        [Required]
        public string EmployeeID { get; set; }

        // Navigation property
        [JsonIgnore]
        [ForeignKey("EmployeeID")]
        public virtual EmployeeProfile Employee { get; set; }
    }
}