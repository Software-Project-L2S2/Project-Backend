using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EmployeeProfileAPI.Models
{
    public class ProfileSkill
    {
        [Key]
        public int SkillID { get; set; }

        [Required]
        public string SkillName { get; set; }
        public string Description { get; set; }
        public int Level { get; set; }

        // Foreign Key to EmployeeProfile
        [Required]
        public string EmployeeID { get; set; }

        // Navigation property - The [JsonIgnore] is crucial to prevent circular references
        // when serializing data back to the client.
        [JsonIgnore]
        [ForeignKey("EmployeeID")]
        public virtual EmployeeProfile Employee { get; set; }
    }
}