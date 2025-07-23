using System;
using System.ComponentModel.DataAnnotations;

namespace HRWorkForceSystemBackend.DTOs.WorkforceDTOs
{
    public class AttritionDto
    {
        // These properties match what your React frontend table needs
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public DateTime ExitDate { get; set; }
        public string Reason { get; set; }
        public string Notes { get; set; }
    }
}