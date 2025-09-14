// You might need to add a namespace that matches your project structure
// For example: namespace YourProjectName.Models;

public class EmployeeCreateModel
{
    // These properties MUST match the keys in the 'employeeData'
    // object you create in your React code. Case matters!

    public string Name { get; set; }
    public string Designation { get; set; }
    public string Department { get; set; }
    public string Gender { get; set; }
    public DateTime? StartDate { get; set; }
    public int? Age { get; set; }
    public string Contact { get; set; }
    public string Email { get; set; }

    // These are the most important properties for linking the tables
    public string UserId { get; set; } // Matches 'userId' from your React object
    public string Role { get; set; }   // Matches 'role' from your React object
}