#nullable enable // This correctly enables safety checks for this file.

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRWorkForceSystemBackend.Models.SkillsModels;
using System.Text.Json.Serialization;

namespace HRWorkForceSystemBackend.Models.WorkforceModels
{
    [Table("Attrition")]
    public class Attrition
    {
        public int Id { get; set; }

        // --- FIX #1: Add the 'required' modifier ---
        // This tells the compiler that EmployeeID is a mandatory field.
        [Required]
        public required int EmployeeID { get; set; }

        // --- FIX #2: Add the 'required' modifier ---
        // This fixes the exact error you are seeing for the 'Reason' property.
        [Required]
        public required string Reason { get; set; } // "Resignation", "Retirement", "Termination"

        // This is already correct. `string?` means it's okay for Details to be null.
        public string? Details { get; set; }

        // --- FIX #3: Add the 'required' modifier ---
        // The date of attrition is also a mandatory field.
        [Required]
        public required DateTime AttritionDate { get; set; }

        // --- FIX #4: Use the 'null forgiving' operator for navigation properties ---
        // This tells the compiler, "Don't worry about this being null right away,
        // Entity Framework Core will handle loading it."
        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; } = null!;
    }
}