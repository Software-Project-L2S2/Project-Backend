using System;
using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.DTOs.WorkforceDTOs
{
    public class MovementDto
    {
        // These properties match what your React frontend table needs
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Status { get; set; } // This will hold the MovementType
        public DateTime Date { get; set; } // This will hold the EffectiveDate
    
        public string NewPosition { get; set; } // This will hold the NewPosition
        public string Description { get; set; } // This will hold the Description
    }
}