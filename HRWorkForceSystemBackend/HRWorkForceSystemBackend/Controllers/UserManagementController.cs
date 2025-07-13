using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.DTOs.UserManagementDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using HRWorkForceSystemBackend.Models.UserMoreDetailModels;
using System.Security.Claims; // Required for getting user claims

namespace HRWorkForceSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, HR")] // Apply to the whole controller
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
            var adminUsers = await _context.Admins
                .Select(a => new RegisteredUserDto
                {
                    Role = "Admin",
                    FirstName = a.FirstName,
                    LastName = a.LastName,
                    Email = a.Email,
                    PhoneNumber = a.PhoneNumber
                }).ToListAsync();

            var hrUsers = await _context.HRUsers
                .Select(h => new RegisteredUserDto
                {
                    Role = "HR",
                    FirstName = h.FirstName,
                    LastName = h.LastName,
                    Email = h.Email,
                    PhoneNumber = h.PhoneNumber
                }).ToListAsync();

            var workforceUsers = await _context.WorkforceUsers
                .Select(w => new RegisteredUserDto
                {
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

        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser([FromQuery] string email, [FromQuery] string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
                return BadRequest("Email and role are required.");

            // Security Check: HR cannot delete an Admin
            var requestingUserRole = User.FindFirstValue(ClaimTypes.Role);
            if (requestingUserRole == "HR" && role.Trim().ToLowerInvariant() == "admin")
            {
                return Forbid("HR users cannot delete Admin users.");
            }

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

                default:
                    return BadRequest("Invalid role.");
            }

            await _context.SaveChangesAsync();
            return Ok("User deleted successfully.");
        }

        [HttpPost("add-workforce-details")]
        public async Task<IActionResult> AddWorkforceDetails([FromBody] WorkforceDetailsDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
                return BadRequest("Email is required.");

            var workforceUser = await _context.WorkforceUsers.FirstOrDefaultAsync(w => w.Email == dto.Email);
            if (workforceUser == null)
                return NotFound("Workforce user not found.");

            if (await _context.WorkforceProfiles.AnyAsync(p => p.WorkforceUserId == workforceUser.Id))
                return BadRequest("Workforce profile already exists.");

            if (await _context.WorkforceProfiles.AnyAsync(p => p.WorkforceId == dto.WorkforceId))
                return BadRequest("WorkforceId already exists. Provide a unique one.");

            var profile = new WorkforceProfile
            {
                WorkforceId = dto.WorkforceId,
                Department = dto.Department,
                JobTitle = dto.JobTitle,
                JobCategory = dto.JobCategory,
                WorkforceUserId = workforceUser.Id
            };

            await _context.WorkforceProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Workforce profile added successfully.", WorkforceId = dto.WorkforceId });
        }

        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Role))
                return BadRequest("Email and role are required.");

            // Security Check: HR cannot update an Admin
            var requestingUserRole = User.FindFirstValue(ClaimTypes.Role);
            if (requestingUserRole == "HR" && request.Role.Trim().ToLowerInvariant() == "admin")
            {
                return Forbid("HR users cannot update Admin users.");
            }

            var normalizedRole = request.Role.Trim().ToLowerInvariant();

            switch (normalizedRole)
            {
                case "admin":
                    var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == request.Email);
                    if (admin == null) return NotFound("Admin user not found.");
                    admin.FirstName = request.FirstName;
                    admin.LastName = request.LastName;
                    admin.PhoneNumber = request.PhoneNumber;
                    break;
                case "hr":
                    var hr = await _context.HRUsers.FirstOrDefaultAsync(h => h.Email == request.Email);
                    if (hr == null) return NotFound("HR user not found.");
                    hr.FirstName = request.FirstName;
                    hr.LastName = request.LastName;
                    hr.PhoneNumber = request.PhoneNumber;
                    break;
                case "workforce":
                    var workforce = await _context.WorkforceUsers.FirstOrDefaultAsync(w => w.Email == request.Email);
                    if (workforce == null) return NotFound("Workforce user not found.");
                    workforce.FirstName = request.FirstName;
                    workforce.LastName = request.LastName;
                    workforce.PhoneNumber = request.PhoneNumber;
                    break;
                default:
                    return BadRequest("Invalid role.");
            }

            await _context.SaveChangesAsync();
            return Ok("User updated successfully.");
        }

        [HttpPost("add-hr-profile")]
        public async Task<IActionResult> AssignHrProfile([FromBody] HrDetailsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            var hrUser = await _context.HRUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (hrUser == null)
                return NotFound("HR user not found.");

            if (await _context.HRProfiles.AnyAsync(p => p.HRUserId == hrUser.Id))
                return BadRequest("HR profile already exists.");

            if (await _context.HRProfiles.AnyAsync(p => p.HRId == dto.HRId))
                return BadRequest("HRId already exists. Provide a unique one.");

            var profile = new HRProfile
            {
                HRUserId = hrUser.Id,
                HRId = dto.HRId,
                Department = dto.Department,
                Gender = dto.Gender,
                StartDate = dto.StartDate,
                Contact = dto.Contact,
                Accounts = dto.Accounts
            };

            await _context.HRProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "HR profile added successfully.", HRId = dto.HRId });
        }
        // Add this method inside the UserManagementController class

        [HttpGet("total-count")] // The route will be /api/UserManagement/total-count
        [Authorize(Roles = "Admin, HR")] // Ensure only Admin and HR can access this
        public async Task<IActionResult> GetTotalUsersCount()
        {
            // Asynchronously get the count from each user table
            var adminCount = await _context.Admins.CountAsync();
            var hrCount = await _context.HRUsers.CountAsync();
            var workforceCount = await _context.WorkforceUsers.CountAsync();

            // Sum the counts to get the total
            var totalEmployees = adminCount + hrCount + workforceCount;

            // Return the result in a simple JSON object
            return Ok(new { totalEmployees });
        }
    }
}