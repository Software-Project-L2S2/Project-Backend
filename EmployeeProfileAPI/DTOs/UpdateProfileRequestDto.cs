using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace EmployeeProfileAPI.DTOs
{
    public class UpdateProfileRequest
    {
        [JsonPropertyName("fullName")]
        [Required]
        public string FullName { get; set; }

        [JsonPropertyName("department")]
        [Required]
        public string Department { get; set; }

        [JsonPropertyName("email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [JsonPropertyName("skillLevel")]
        public string SkillLevel { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }

        [JsonPropertyName("projectsCompleted")]
        public int ProjectsCompleted { get; set; }
    }
}
