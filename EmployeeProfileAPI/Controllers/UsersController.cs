using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using EmployeeProfileAPI.DTOs.UserDTOs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeProfileAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/Users
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == userDto.Email.ToLower());

                if (existingUser != null)
                {
                    return Conflict(new { message = "User with this email already exists" });
                }

                var user = new User
                {
                    FirstName = userDto.FirstName?.Trim(),
                    LastName = userDto.LastName?.Trim(),
                    Email = userDto.Email?.Trim().ToLower(),
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
                    PhoneNumber = userDto.PhoneNumber?.Trim(),
                    Role = userDto.Role?.Trim(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var result = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.CreatedAt
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the detailed error for debugging
                Console.WriteLine($"Error creating user: {ex.ToString()}");
                // Return a generic error message to the client
                return StatusCode(500, new { message = "An unexpected error occurred while creating the user.", error = ex.Message });
            }
        }

        // GET: api/Users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                var result = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.CreatedAt,
                    user.UpdatedAt
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching user: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching user", error = ex.Message });
            }
        }

        // GET: api/Users
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(u => new
                    {
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        u.PhoneNumber,
                        u.Role,
                        u.CreatedAt,
                        u.UpdatedAt
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching users: {ex.Message}");
                return StatusCode(500, new { message = "Error fetching users", error = ex.Message });
            }
        }

        // PUT: api/Users/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                user.FirstName = userDto.FirstName?.Trim() ?? user.FirstName;
                user.LastName = userDto.LastName?.Trim() ?? user.LastName;
                user.PhoneNumber = userDto.PhoneNumber?.Trim() ?? user.PhoneNumber;
                user.Role = userDto.Role?.Trim() ?? user.Role;
                user.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(userDto.Email) &&
                    user.Email.ToLower() != userDto.Email.ToLower())
                {
                    var emailExists = await _context.Users
                        .AnyAsync(u => u.Email.ToLower() == userDto.Email.ToLower() && u.Id != id);

                    if (emailExists)
                    {
                        return Conflict(new { message = "Email already exists" });
                    }

                    user.Email = userDto.Email.Trim().ToLower();
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                var result = new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.Role,
                    user.UpdatedAt
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating user: {ex.Message}");
                return StatusCode(500, new { message = "Error updating user", error = ex.Message });
            }
        }

        // DELETE: api/Users/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user: {ex.Message}");
                return StatusCode(500, new { message = "Error deleting user", error = ex.Message });
            }
        }
    }
}