using System.ComponentModel.DataAnnotations;
using HRWorkForceSystemBackend.Models.UserMoreDetailModels;

namespace HRWorkForceSystemBackend.Models.AuthModels;

public class HRUser
{
     
    public int Id { get; set; }


    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;
    // public string Department { get; set; }


    public HRProfile HRProfile { get; set; }
}
