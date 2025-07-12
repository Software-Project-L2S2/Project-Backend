using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.DTOs.UserManagementDTOs; // IMPORTANT: Ensure you are using the correct namespace
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using EmployeeProfileAPI.Models.UserMoreDetailModels;

namespace EmployeeProfileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserManagementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserManagementController(AppDbContext context)
        {
            _context = context;
        }

        // THIS METHOD IS NOW FIXED TO SEND THE USER ID
        [HttpGet("registered-users")]
        public async Task<IActionResult> GetRegisteredUsers()
        {
            var adminUsers = await _context.Admins
                .Select(a => new UserViewDto // Use the new DTO that has the 'Id' property
                {
                    Id = a.Id, // Include the ID
                    Role = "Admin",
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber
                }).ToListAsync();

            var hrUsers = await _context.HRUsers
                .Select(h => new UserViewDto // Use the new DTO
                {
                    Id = h.Id, // Include the ID
                    Role = "HR",
                    FirstName = h.FirstName,
                    LastName = h.LastName,
                    Email = h.Email,
                    PhoneNumber = h.PhoneNumber
                }).ToListAsync();

            var workforceUsers = await _context.WorkforceUsers
                .Select(w => new UserViewDto // Use the new DTO
                {
                    Id = w.Id, // Include the ID
                    Role = "Workforce",
                    FirstName = w.FirstName,
                    LastName = w.LastName,
                    Email = w.Email,
                    PhoneNumber = w.PhoneNumber
                }).ToListAsync();

            var allUsers = adminUsers
                .Concat(hrUsers)
                .Concat(workforceUsers)
                .OrderBy(u => u.FirstName)
                .ToList();

            return Ok(allUsers);
        }
        
        // --- ALL OTHER METHODS IN THIS CONTROLLER ARE UNCHANGED ---

        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser([FromQuery] string email, [FromQuery] string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
                return BadRequest("Email and role are required.");
            var normalizedRole = role.Trim().ToLowerInvariant();
            switch (normalizedRole) {
                case "admin": var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == email); if (admin == null) return NotFound("Admin user not found."); _context.Admins.Remove(admin); break;
                case "hr": var hr = await _context.HRUsers.FirstOrDefaultAsync(h => h.Email == email); if (hr == null) return NotFound("HR user not found."); _context.HRUsers.Remove(hr); break;
                case "workforce": var workforce = await _context.WorkforceUsers.FirstOrDefaultAsync(w => w.Email == email); if (workforce == null) return NotFound("Workforce user not found."); _context.WorkforceUsers.Remove(workforce); break;
                default: return BadRequest("Invalid role.");
            }
            await _context.SaveChangesAsync();
            return Ok("User deleted successfully.");
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Role))
                return BadRequest("Email and role are required.");
            var normalizedRole = request.Role.Trim().ToLowerInvariant();
            switch (normalizedRole) {
                case "admin": var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == request.Email); if (admin == null) return NotFound("Admin user not found."); admin.FirstName = request.FirstName; admin.LastName = request.LastName; admin.PhoneNumber = request.PhoneNumber; break;
                case "hr": var hr = await _context.HRUsers.FirstOrDefaultAsync(h => h.Email == request.Email); if (hr == null) return NotFound("HR user not found."); hr.FirstName = request.FirstName; hr.LastName = request.LastName; hr.PhoneNumber = request.PhoneNumber; break;
                case "workforce": var workforce = await _context.WorkforceUsers.FirstOrDefaultAsync(w => w.Email == request.Email); if (workforce == null) return NotFound("Workforce user not found."); workforce.FirstName = request.FirstName; workforce.LastName = request.LastName; workforce.PhoneNumber = request.PhoneNumber; break;
                default: return BadRequest("Invalid role.");
            }
            await _context.SaveChangesAsync();
            return Ok("User updated successfully.");
        }
    }
}