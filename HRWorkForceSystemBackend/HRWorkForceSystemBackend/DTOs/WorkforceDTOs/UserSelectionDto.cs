using System;
using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.DTOs.WorkforceDTOs
{
    public class UserSelectionDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; } // "Employee", "Admin", "HR"
    }
}