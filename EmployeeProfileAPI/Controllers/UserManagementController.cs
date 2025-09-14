using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System;
using BCrypt.Net;
using System.Collections.Generic;
using EmployeeProfileAPI.Models.AuthModels;// Required for List and Dictionary

namespace EmployeeProfileAPI.Controllers
{
    // --- DTO (Data Transfer Object) DEFINITIONS ---

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
        [Required] [EmailAddress] public string Email { get; set; }
        [Required] public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }

    // MODIFIED DTO: Includes a 'hasProfile' flag.
    public class RegisteredUserDto
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool HasProfile { get; set; } // True if an employee profile exists for this email
    }
    
    public class AddEmployeeDto
    {
        [Required] public string Name { get; set; }
        [Required] public string Designation { get; set; }
        [Required] public string Department { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public DateTime StartDate { get; set; }
        [Required] public int Age { get; set; }
        [Required] public string Contact { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
    }

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

        // GET: api/UserManagement/users
        // UPDATED: This now efficiently checks for employee profiles.
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var admins = await _context.Admins.AsNoTracking().ToListAsync();
            var hrs = await _context.HRUsers.AsNoTracking().ToListAsync();
            var workforces = await _context.WorkforceUsers.AsNoTracking().ToListAsync();

            var allUserEmails = admins.Select(u => u.Email)
                                      .Concat(hrs.Select(u => u.Email))
                                      .Concat(workforces.Select(u => u.Email))
                                      .Distinct()
                                      .ToList();

            // Create a HashSet of emails that exist in the Employees table for fast lookups.
            var emailsWithProfiles = (await _context.Employees
                .Where(e => allUserEmails.Contains(e.Email))
                .Select(e => e.Email)
                .ToListAsync())
                .ToHashSet();

            var adminDtos = admins.Select(u => new RegisteredUserDto
            {
                Id = u.Id, FirstName = u.FirstName, LastName = u.LastName, Email = u.Email, PhoneNumber = u.PhoneNumber, Role = "Admin",
                HasProfile = emailsWithProfiles.Contains(u.Email)
            });

            var hrDtos = hrs.Select(u => new RegisteredUserDto
            {
                Id = u.Id, FirstName = u.FirstName, LastName = u.LastName, Email = u.Email, PhoneNumber = u.PhoneNumber, Role = "HR",
                HasProfile = emailsWithProfiles.Contains(u.Email)
            });

            var workforceDtos = workforces.Select(u => new RegisteredUserDto
            {
                Id = u.Id, FirstName = u.FirstName, LastName = u.LastName, Email = u.Email, PhoneNumber = u.PhoneNumber, Role = "Workforce",
                HasProfile = emailsWithProfiles.Contains(u.Email)
            });

            var allUsers = adminDtos.Concat(hrDtos).Concat(workforceDtos).OrderBy(u => u.FirstName).ToList();
            return Ok(allUsers);
        }

        // POST: api/UserManagement/create-user
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var emailExists = await _context.Admins.AnyAsync(u => u.Email == dto.Email) ||
                              await _context.HRUsers.AnyAsync(u => u.Email == dto.Email) ||
                              await _context.WorkforceUsers.AnyAsync(u => u.Email == dto.Email);

            if (emailExists) return BadRequest(new { message = "Email address is already registered as a user." });

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            switch (dto.Role.ToLowerInvariant())
            {
                case "admin": _context.Admins.Add(new Admin { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email, PasswordHash = hashedPassword, PhoneNumber = dto.PhoneNumber }); break;
                case "hr": _context.HRUsers.Add(new HRUser { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email, PasswordHash = hashedPassword, PhoneNumber = dto.PhoneNumber }); break;
                case "workforce": _context.WorkforceUsers.Add(new WorkforceUser { FirstName = dto.FirstName, LastName = dto.LastName, Email = dto.Email, PasswordHash = hashedPassword, PhoneNumber = dto.PhoneNumber }); break;
                default: return BadRequest("Invalid role specified.");
            }
            await _context.SaveChangesAsync();
            return Ok(new { message = "User created successfully." });
        }
        
        // DELETE: api/UserManagement/delete-user
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
            return Ok(new { message = "User deleted successfully." });
        }
        
        // PUT: api/UserManagement/update-user
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Role)) return BadRequest("Email and role are required.");
            var normalizedRole = request.Role.Trim().ToLowerInvariant();
            dynamic user = null;
            switch (normalizedRole)
            {
                case "admin": user = await _context.Admins.FirstOrDefaultAsync(a => a.Email == request.Email); break;
                case "hr": user = await _context.HRUsers.FirstOrDefaultAsync(h => h.Email == request.Email); break;
                case "workforce": user = await _context.WorkforceUsers.FirstOrDefaultAsync(w => w.Email == request.Email); break;
                default: return BadRequest("Invalid role.");
            }
            if (user == null) return NotFound("User not found.");
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            await _context.SaveChangesAsync();
            return Ok(new { message = "User updated successfully." });
        }

        // POST: api/UserManagement/add-employee-details
        [HttpPost("add-employee-details")]
        public async Task<IActionResult> AddEmployeeDetails([FromBody] AddEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var employeeExists = await _context.Employees.AnyAsync(e => e.Email == dto.Email);
            if(employeeExists)
            {
                return Conflict(new { message = "An employee profile with this email already exists." });
            }
            var newEmployee = new Employee 
            {
                Name = dto.Name, Designation = dto.Designation, Department = dto.Department, Gender = dto.Gender,
                StartDate = dto.StartDate, Age = dto.Age, Contact = dto.Contact, Email = dto.Email
            };
            _context.Employees.Add(newEmployee); 
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(EmployeeController.GetEmployee), "Employee", new { id = newEmployee.EmployeeID }, newEmployee);
        }
    }
}