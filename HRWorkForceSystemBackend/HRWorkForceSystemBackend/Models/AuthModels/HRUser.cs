// D:\Projects\Softwareproject_UOM_2\Project-Backend\HRWorkForceSystemBackend\HRWorkForceSystemBackend\Models\AuthModels\HRUser.cs

using System.ComponentModel.DataAnnotations; // Add this for [StringLength]
using HRWorkForceSystemBackend.Models.SkillgapModels;

namespace HRWorkForceSystemBackend.Models.AuthModels
{
    public class HRUser
    {
        public int Id { get; set; }

        [Required] // Added Required for FirstName
        [StringLength(100)] // Added StringLength
        public string FirstName { get; set; } = string.Empty;

        [Required] // Added Required for LastName
        [StringLength(100)] // Added StringLength
        public string LastName { get; set; } = string.Empty;

        // NEW: FullName property (can be a computed property or a stored one)
        // For simplicity and to match previous seeding, let's make it a direct property.
        // You could also make it a computed property: public string FullName => $"{FirstName} {LastName}";
        [StringLength(200)]
        public string FullName { get; set; } = string.Empty; // Added FullName

        [Required] // Added Required for Email
        [EmailAddress] // Added EmailAddress attribute for validation
        [StringLength(256)] // Added StringLength
        public string Email { get; set; } = string.Empty;

        [Required] // Added Required for PasswordHash
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(20)] // Added StringLength
        public string PhoneNumber { get; set; } = string.Empty;

        // NEW: Properties for Department and Role
        [StringLength(100)]
        public string Department { get; set; } = string.Empty; // Added Department

        [StringLength(100)]
        public string Role { get; set; } = string.Empty; // Added Role

        // NEW: UserName property (used in seeding data for HRUser)
        [Required] // Added Required for UserName
        [StringLength(256)] // Added StringLength
        public string UserName { get; set; } = string.Empty; // Added UserName

        public virtual ICollection<EmployeeSkill> EmployeeSkills { get; set; } = new List<EmployeeSkill>();
    }
}