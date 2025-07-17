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
using HRWorkForceSystemBackend.Models.AuthModels; // This will find Admin, HRUser, WorkforceUser
using HRWorkForceSystemBackend.Models;
using HRWorkForceSystemBackend.Models.SkillsModels;


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
            var allUsersQuery = _context.Admins.Select(u => new { u.Email, u.Id, u.FirstName, u.LastName, u.PhoneNumber, Role = "Admin" })
                .Concat(_context.HRUsers.Select(u => new { u.Email, u.Id, u.FirstName, u.LastName, u.PhoneNumber, Role = "HR" }))
                .Concat(_context.WorkforceUsers.Select(u => new { u.Email, u.Id, u.FirstName, u.LastName, u.PhoneNumber, Role = "Workforce" }));

            var allUsers = await allUsersQuery.AsNoTracking().ToListAsync();

            var userEmails = allUsers.Select(u => u.Email).ToList();
            var emailsWithProfiles = (await _context.Employees
                .Where(e => userEmails.Contains(e.Email))
                .Select(e => e.Email)
                .ToListAsync())
                .ToHashSet();

            var result = allUsers.Select(u => new RegisteredUserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                HasProfile = emailsWithProfiles.Contains(u.Email)
            }).OrderBy(u => u.FirstName).ToList();

            return Ok(result);
        }

           // In UserManagementController.cs

[HttpPost("create-user")]
[Authorize(Roles = "Admin, HR")] // <-- STEP 1: Change this to allow HR access
public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
{
    if (!ModelState.IsValid) return BadRequest(ModelState);

    // --- STEP 2: Add this security logic block ---
    var requestingUserRole = User.FindFirstValue(ClaimTypes.Role);
    if (requestingUserRole == "HR" && !dto.Role.Equals("workforce", StringComparison.InvariantCultureIgnoreCase))
    {
        return Forbid("HR users can only create Workforce users.");
    }
    // --- End of new logic block ---

    var emailExists = await _context.Admins.AnyAsync(u => u.Email == dto.Email) ||
                      await _context.HRUsers.AnyAsync(u => u.Email == dto.Email) ||
                      await _context.WorkforceUsers.AnyAsync(u => u.Email == dto.Email);

    if (emailExists) return BadRequest(new { message = "Email address is already registered." });

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

                // Security check from the first file.
                var requestingUserRole = User.FindFirstValue(ClaimTypes.Role);
                if (requestingUserRole == "HR" && request.Role.Trim().ToLowerInvariant() == "admin")
                {
                    return Forbid("HR users cannot update Admin users.");
                }

                dynamic user = null;
                switch (request.Role.Trim().ToLowerInvariant())
                {
                    case "admin": user = await _context.Admins.FirstOrDefaultAsync(u => u.Email == request.Email); break;
                    case "hr": user = await _context.HRUsers.FirstOrDefaultAsync(u => u.Email == request.Email); break;
                    case "workforce": user = await _context.WorkforceUsers.FirstOrDefaultAsync(u => u.Email == request.Email); break;
                    default: return BadRequest("Invalid role.");
                }

                if (user == null) return NotFound("User not found.");

                user.FirstName = request.FirstName;
                user.LastName = request.LastName;
                user.PhoneNumber = request.PhoneNumber;

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