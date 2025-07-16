using System.ComponentModel.DataAnnotations;
using HRWorkForceSystemBackend.Models.TrainingProgramModels;


namespace HRWorkForceSystemBackend.Models.AuthModels;

public class HRUser
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Phone]
    [StringLength(15)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Employee;

    // Account Status
    public bool IsActive { get; set; } = true;
    public bool IsEmailVerified { get; set; } = false;
    
    // Audit Fields
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // HR Specific Fields (optional - mainly for Employee role)
    public string EmployeeId { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public DateTime? HireDate { get; set; }

    // Navigation Properties
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

    // Computed Properties
    public string FullName => $"{FirstName} {LastName}";
    public bool IsAdmin => Role == UserRole.Admin;
    public bool IsHR => Role == UserRole.HR;
    public bool IsEmployee => Role == UserRole.Employee;
}

public enum UserRole
{
    Admin = 1,
    HR = 2,
    Employee = 3
}