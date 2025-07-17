using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace HRWorkForceSystemBackend.Models.LeaveModels
{
    public class EmployeeLeave
    {
        [Key]
        public string EmployeeId { get; set; }
        public int TotalLeaves { get; set; } = 15;
        public int LeavesTaken { get; set; } = 0;
        public int AvailableLeaves => TotalLeaves - LeavesTaken;
    }
}