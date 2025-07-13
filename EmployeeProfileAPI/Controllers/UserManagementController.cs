using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using EmployeeProfileAPI.Models.AuthModels;
using EmployeeProfileAPI.Models.UserMoreDetailModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;

namespace EmployeeProfileAPI.Controllers
{
    // --- DTO DEFINITIONS ---
    // You can move these to a separate DTOs folder if you prefer.

    public class CreateUserDto
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
        [Required] public string Password { get; set; }
        public string PhoneNumber { get; set; }
        [Required] public string Role { get; set; }
    }

    public class UpdateUserDto
    {
        [Required] public string Email { get; set; }
        [Required] public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
    
    // NEW DTO: This is the data structure that will be sent to the frontend user table.
    public class RegisteredUserWithEmployeeIdDto
    {
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int? EmployeeId { get; set; } // The EmployeeID from the Employees table.
    }

    // --- CONTROLLER ---
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserManagementController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("registered-users")]
        public async Task<IActionResult> GetRegisteredUsers()
        {
            // This logic performs a join to find the matching EmployeeID for each user based on their email.
            var adminUsers = await _context.Admins
                .GroupJoin(_context.Employees, u => u.Email, e => e.Email, (user, employees) => new { user, employees })
                .SelectMany(x => x.employees.DefaultIfEmpty(),
                    (x, employee) => new RegisteredUserWithEmployeeIdDto {
                        Role = "Admin", FirstName = x.user.FirstName, LastName = x.user.LastName, Email = x.user.Email, PhoneNumber = x.user.PhoneNumber,
                        EmployeeId = employee != null ? (int?)employee.EmployeeID : null
                    }).ToListAsync();

            var hrUsers = await _context.HRUsers
                .GroupJoin(_context.Employees, u => u.Email, e => e.Email, (user, employees) => new { user, employees })
                .SelectMany(x => x.employees.DefaultIfEmpty(),
                    (x, employee) => new RegisteredUserWithEmployeeIdDto {
                        Role = "HR", FirstName = x.user.FirstName, LastName = x.user.LastName, Email = x.user.Email, PhoneNumber = x.user.PhoneNumber,
                        EmployeeId = employee != null ? (int?)employee.EmployeeID : null
                    }).ToListAsync();

            var workforceUsers = await _context.WorkforceUsers
                .GroupJoin(_context.Employees, u => u.Email, e => e.Email, (user, employees) => new { user, employees })
                .SelectMany(x => x.employees.DefaultIfEmpty(),
                    (x, employee) => new RegisteredUserWithEmployeeIdDto {
                        Role = "Workforce", FirstName = x.user.FirstName, LastName = x.user.LastName, Email = x.user.Email, PhoneNumber = x.user.PhoneNumber,
                        EmployeeId = employee != null ? (int?)employee.EmployeeID : null
                    }).ToListAsync();

            var allUsers = adminUsers.Concat(hrUsers).Concat(workforceUsers).ToList();
            return Ok(allUsers);
        }

        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var emailExists = await _context.Admins.AnyAsync(u => u.Email == dto.Email) ||
                              await _context.HRUsers.AnyAsync(u => u.Email == dto.Email) ||
                              await _context.WorkforceUsers.AnyAsync(u => u.Email == dto.Email);

            if (emailExists) return BadRequest("Email address is already in use.");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            switch (dto.Role.ToLower())
            {
                case "admin": _context.Admins.Add(new Admin { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email, PasswordHash = hashedPassword, PhoneNumber = dto.PhoneNumber }); break;
                case "hr": _context.HRUsers.Add(new HRUser { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email, PasswordHash = hashedPassword, PhoneNumber = dto.PhoneNumber }); break;
                case "workforce": _context.WorkforceUsers.Add(new WorkforceUser { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email, PasswordHash = hashedPassword, PhoneNumber = dto.PhoneNumber }); break;
                default: return BadRequest("Invalid role specified.");
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "User created successfully." });
        }
        
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser([FromQuery] string email, [FromQuery] string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role)) return BadRequest("Email and role are required.");
            var normalizedRole = role.Trim().ToLowerInvariant();
            switch (normalizedRole)
            {
                case "admin":
                    var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email);
                    if (admin == null) return NotFound("Admin user not found.");
                    _context.Admins.Remove(admin);
                    break;
                case "hr":
                    var hr = await _context.HRUsers.FirstOrDefaultAsync(h => h.Email == email);
                    if (hr == null) return NotFound("HR user not found.");
                    _context.HRUsers.Remove(hr);
                    break;
                case "workforce":
                    var workforce = await _context.WorkforceUsers.FirstOrDefaultAsync(w => w.Email == email);
                    if (workforce == null) return NotFound("Workforce user not found.");
                    _context.WorkforceUsers.Remove(workforce);
                    break;
                default: return BadRequest("Invalid role.");
            }
            await _context.SaveChangesAsync();
            return Ok("User deleted successfully.");
        }
        
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Role)) return BadRequest("Email and role are required.");
            var normalizedRole = request.Role.Trim().ToLowerInvariant();
            switch (normalizedRole)
            {
                case "admin":
                    var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == request.Email);
                    if (admin == null) return NotFound("Admin user not found.");
                    admin.FirstName = request.FirstName; admin.LastName = request.LastName; admin.PhoneNumber = request.PhoneNumber;
                    break;
                case "hr":
                    var hr = await _context.HRUsers.FirstOrDefaultAsync(h => h.Email == request.Email);
                    if (hr == null) return NotFound("HR user not found.");
                    hr.FirstName = request.FirstName; hr.LastName = request.LastName; hr.PhoneNumber = request.PhoneNumber;
                    break;
                case "workforce":
                    var workforce = await _context.WorkforceUsers.FirstOrDefaultAsync(w => w.Email == request.Email);
                    if (workforce == null) return NotFound("Workforce user not found.");
                    workforce.FirstName = request.FirstName; workforce.LastName = request.LastName; workforce.PhoneNumber = request.PhoneNumber;
                    break;
                default:
                    return BadRequest("Invalid role.");
            }
            await _context.SaveChangesAsync();
            return Ok("User updated successfully.");
        }
    }
}