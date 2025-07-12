using HRWorkForceSystemBackend.Models.UserMoreDetailModels;
using Microsoft.EntityFrameworkCore;

namespace HRWorkForceSystemBackend.Models.AuthModels;

public class WorkforceUser
{
    public int Id { get; set; }
   

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

   
   // public string? WorkforceId { get; set; }
   // public string Department {  get; set; } = string.Empty;
   // public string CurrentRole { get; set; }

    public WorkforceProfile WorkforceProfiles{ get; set; }
}
