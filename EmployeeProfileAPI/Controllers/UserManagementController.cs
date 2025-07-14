using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models; // Ensure your Employee model is in this namespace
using EmployeeProfileAPI.Models.AuthModels; // Ensure your Auth models are here
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.ComponentModel.DataAnnotations;
using System; // Required for DateTime
using BCrypt.Net; // You need BCrypt.Net for password hashing

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
        [Required] public string Email { get; set; }
        [Required] public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class RegisteredUserDto
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
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
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var admins = await _context.Admins.Select(u => new RegisteredUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = "Admin"
            }).ToListAsync();

            var hrs = await _context.HRUsers.Select(u => new RegisteredUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = "HR"
            }).ToListAsync();

            var workforces = await _context.WorkforceUsers.Select(u => new RegisteredUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = "Workforce"
            }).ToListAsync();

            var allUsers = admins.Concat(hrs).Concat(workforces).ToList();
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
            return Ok("User deleted successfully.");
        }
        
        // PUT: api/UserManagement/update-user
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

        // POST: api/UserManagement/add-employee-details
        [HttpPost("add-employee-details")]
        public async Task<IActionResult> AddEmployeeDetails([FromBody] AddEmployeeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // This code assumes your DB model class is `Employee`
            var newEmployee = new Employee 
            {
                Name = dto.Name,
                Designation = dto.Designation,
                Department = dto.Department,
                Gender = dto.Gender,
                StartDate = dto.StartDate,
                Age = dto.Age,
                Contact = dto.Contact,
                Email = dto.Email,
                // The database should auto-generate the EmployeeID.
                // ProfileImage and CompanyLogo will be null by default.
            };

            // This code assumes your DbSet in your AppDbContext is called `Employees`
            _context.Employees.Add(newEmployee); 
            await _context.SaveChangesAsync();

            return Ok(new { message = "Employee details added successfully." });
        }
    }
}