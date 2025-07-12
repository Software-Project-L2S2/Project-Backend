using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.DTOs.UserManagementDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using HRWorkForceSystemBackend.Models.UserMoreDetailModels;

namespace HRWorkForceSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {


        private readonly AppDbContext _context;

        public UserManagementController(AppDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Admin")]
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
                .ToList();

            return Ok(allUsers);
        }



        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser([FromQuery] string email, [FromQuery] string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
                return BadRequest("Email and role are required.");

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




        //private async Task<string> GenerateUniqueWorkforceIdAsync()
        //{
        //    const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //    int count = await _context.WorkforceUsers.CountAsync(w => w.WorkforceId != null) + 1;
        //    string padded = count.ToString("D3");
        //    string randomChar = chars[new Random().Next(chars.Length)].ToString();

        //    string generatedId = padded + randomChar;

        //    while (await _context.WorkforceUsers.AnyAsync(w => w.WorkforceId == generatedId))
        //    {
        //        randomChar = chars[new Random().Next(chars.Length)].ToString();
        //        generatedId = padded + randomChar;
        //    }

        //    return generatedId;
        //}
        [Authorize(Roles = "Admin")]
        [HttpPost("add-workforce-details")]
        public async Task<IActionResult> AddWorkforceDetails([FromBody] WorkforceDetailsDto dto)
        {
            if (string.IsNullOrEmpty(dto.Email))
                return BadRequest("Email is required.");

            // Find the WorkforceUser based on email
            var workforceUser = await _context.WorkforceUsers
                .FirstOrDefaultAsync(w => w.Email == dto.Email);

            if (workforceUser == null)
                return NotFound("Workforce user not found.");

            // Check if a profile already exists for this user
            var existingProfile = await _context.WorkforceProfiles
                .FirstOrDefaultAsync(p => p.WorkforceUserId == workforceUser.Id);

            if (existingProfile != null)
                return BadRequest("Workforce profile already exists.");

            // Ensure WorkforceId is unique
            bool idExists = await _context.WorkforceProfiles
                .AnyAsync(p => p.WorkforceId == dto.WorkforceId);

            if (idExists)
                return BadRequest("WorkforceId already exists. Provide a unique one.");

            // Create profile using the FK from SQL Server (auto-generated)
            var profile = new WorkforceProfile
            {
                WorkforceId = dto.WorkforceId,
                Department = dto.Department,
                JobTitle = dto.JobTitle,
                JobCategory = dto.JobCategory,
                //StartDate = dto.StartDate,
                WorkforceUserId = workforceUser.Id // 👈 FK generated by S
            };

            await _context.WorkforceProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Workforce profile added successfully.",
                WorkforceId = dto.WorkforceId
            });
        }




        [Authorize(Roles = "Admin")]
        [HttpPut("update-user")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Role))
                return BadRequest("Email and role are required.");

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


        [Authorize(Roles = "Admin")]
        [HttpPost("add-hr-profile")]
        public async Task<IActionResult> AssignHrProfile([FromBody] HrDetailsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            var hrUser = await _context.HRUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (hrUser == null)
                return NotFound("HR user not found.");

            var existingProfile = await _context.HRProfiles
                .FirstOrDefaultAsync(p => p.HRUserId == hrUser.Id);
            if (existingProfile != null)
                return BadRequest("HR profile already exists.");

            var hrIdExists = await _context.HRProfiles
                .AnyAsync(p => p.HRId == dto.HRId);
            if (hrIdExists)
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

            return Ok(new
            {
                Message = "HR profile added successfully.",
                HRId = dto.HRId
            });
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("hr-profiles")]
        public async Task<IActionResult> GetAllHRProfiles()
        {
            var hrProfiles = await _context.HRProfiles
                .Include(p => p.HRUser)
                .Select(p => new
                {
                    HRId = p.HRId,
                    FirstName = p.HRUser.FirstName,
                    LastName = p.HRUser.LastName,
                    Email = p.HRUser.Email,
                    Department = p.Department,
                    Gender = p.Gender,
                    StartDate = p.StartDate,
                    Contact = p.Contact,
                    Accounts = p.Accounts
                })
                .ToListAsync();

            return Ok(hrProfiles);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("workforce-profiles")]
        public async Task<IActionResult> GetAllWorkforceProfiles()
        {
            var workforceProfiles = await _context.WorkforceProfiles
                .Include(p => p.WorkforceUser)
                .Select(p => new
                {
                    WorkforceId = p.WorkforceId,
                    FirstName = p.WorkforceUser.FirstName,
                    LastName = p.WorkforceUser.LastName,
                    Email = p.WorkforceUser.Email,
                    Department = p.Department,
                    JobTitle = p.JobTitle,
                    JobCategory = p.JobCategory
                    // Include more fields if available
                })
                .ToListAsync();

            return Ok(workforceProfiles);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-hr-profile")]
        public async Task<IActionResult> DeleteHRProfile([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var hrUser = await _context.HRUsers.FirstOrDefaultAsync(u => u.Email == email);
            if (hrUser == null)
                return NotFound("HR user not found.");

            var profile = await _context.HRProfiles.FirstOrDefaultAsync(p => p.HRUserId == hrUser.Id);
            if (profile == null)
                return NotFound("HR profile not found.");

            _context.HRProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return Ok("HR profile deleted successfully.");
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-workforce-profile")]
        public async Task<IActionResult> DeleteWorkforceProfile([FromQuery] string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return BadRequest("Email is required.");

            var workforceUser = await _context.WorkforceUsers.FirstOrDefaultAsync(w => w.Email == email);
            if (workforceUser == null)
                return NotFound("Workforce user not found.");

            var profile = await _context.WorkforceProfiles.FirstOrDefaultAsync(p => p.WorkforceUserId == workforceUser.Id);
            if (profile == null)
                return NotFound("Workforce profile not found.");

            _context.WorkforceProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return Ok("Workforce profile deleted successfully.");
        }



        [Authorize(Roles = "Admin")]
        [HttpPut("update-hr-profile")]
        public async Task<IActionResult> UpdateHRProfile([FromBody] HrDetailsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            var hrUser = await _context.HRUsers.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (hrUser == null)
                return NotFound("HR user not found.");

            var profile = await _context.HRProfiles.FirstOrDefaultAsync(p => p.HRUserId == hrUser.Id);
            if (profile == null)
                return NotFound("HR profile not found.");

            profile.Department = dto.Department;
            profile.Gender = dto.Gender;
            profile.StartDate = dto.StartDate;
            profile.Contact = dto.Contact;
            profile.Accounts = dto.Accounts;

            await _context.SaveChangesAsync();

            return Ok("HR profile updated successfully.");
        }





        [Authorize(Roles = "Admin")]
        [HttpPut("update-workforce-profile")]
        public async Task<IActionResult> UpdateWorkforceProfile([FromBody] WorkforceDetailsDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                return BadRequest("Email is required.");

            var workforceUser = await _context.WorkforceUsers.FirstOrDefaultAsync(w => w.Email == dto.Email);
            if (workforceUser == null)
                return NotFound("Workforce user not found.");

            var profile = await _context.WorkforceProfiles.FirstOrDefaultAsync(p => p.WorkforceUserId == workforceUser.Id);
            if (profile == null)
                return NotFound("Workforce profile not found.");

            profile.Department = dto.Department;
            profile.JobTitle = dto.JobTitle;
            profile.JobCategory = dto.JobCategory;

            await _context.SaveChangesAsync();

            return Ok("Workforce profile updated successfully.");
        }

    }



}

