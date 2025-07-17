#nullable enable // Good practice to have this at the top of the file

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HRWorkForceSystemBackend.Models.SkillsModels;

namespace HRWorkForceSystemBackend.Models.WorkforceModels
{
    [Table("Movement")]
    public class Movement
    {
        public int Id { get; set; }

        // This is a foreign key and MUST be provided.
        public required int EmployeeID { get; set; }

        // The type of movement is essential to the record.
        public required string MovementType { get; set; } // "Promotion", "Transfer", "Exit"

        // These properties are genuinely optional. An "Exit" doesn't have a new position.
        public string? OldPosition { get; set; }
        public string? NewPosition { get; set; }
        public string? OldDepartment { get; set; }
        public string? NewDepartment { get; set; }
        public string? Description { get; set; }

        // The date is essential.
        public required DateTime EffectiveDate { get; set; }

        [ForeignKey("EmployeeID")]
        public virtual Employee Employee { get; set; } = null!; // The '!' tells the compiler "Trust me, EF Core will load this"

    }
}