using System;
using System.ComponentModel.DataAnnotations;

namespace EmployeeProfileAPI.DTOs.EmployeeDTOs
{
    public class CreateEmployeeDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Designation { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Department { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Gender { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        [Range(18, 100)]
        public int Age { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Contact { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        
        // Link to the user in the Users table
        public int? UserId { get; set; }
        
        [StringLength(500)]
        public string ProfileImage { get; set; }
        
        [StringLength(500)]
        public string CompanyLogo { get; set; }
    }
    
    public class UpdateEmployeeDto
    {
        [Required]
        public int EmployeeId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Designation { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Department { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Gender { get; set; }
        
        [Required]
        public DateTime StartDate { get; set; }
        
        [Required]
        [Range(18, 100)]
        public int Age { get; set; }
        
        [Required]
        [StringLength(20)]
        public string Contact { get; set; }
        
        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; }
        
        [StringLength(500)]
        public string ProfileImage { get; set; }
        
        [StringLength(500)]
        public string CompanyLogo { get; set; }
    }
    
    public class EmployeeViewDto
    {
        public int EmployeeId { get; set; }
        public string Name { get; set; }
        public string Designation { get; set; }
        public string Department { get; set; }
        public string Gender { get; set; }
        public DateTime StartDate { get; set; }
        public int Age { get; set; }
        public string Contact { get; set; }
        public string Email { get; set; }
        public int? UserId { get; set; }
        public string ProfileImage { get; set; }
        public string CompanyLogo { get; set; }
        
    }
}