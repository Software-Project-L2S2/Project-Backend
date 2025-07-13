using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EmployeeProfileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeProfilesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public EmployeeProfilesController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/EmployeeProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeProfile>>> GetAllEmployeeProfiles()
        {
            try
            {
                var employees = await _context.EmployeeProfiles
                    .Include(e => e.ProfileSkills)
                    .Include(e => e.ProfileEducation)
                    .ToListAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee profiles", error = ex.Message });
            }
        }

        // GET: api/EmployeeProfiles/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeProfile>> GetEmployeeProfile(string id)
        {
            try
            {
                var employee = await _context.EmployeeProfiles
                    .Include(e => e.ProfileSkills)
                    .Include(e => e.ProfileEducation)
                    .FirstOrDefaultAsync(e => e.EmployeeID == id);

                if (employee == null)
                    return NotFound(new { message = $"Employee with ID '{id}' not found." });

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee profile", error = ex.Message });
            }
        }

        // POST: api/EmployeeProfiles
        [HttpPost]
        public async Task<ActionResult<EmployeeProfile>> CreateEmployeeProfile(EmployeeProfile employeeProfile)
        {
            try
            {
                var existingEmployee = await _context.EmployeeProfiles
                    .FirstOrDefaultAsync(e => e.EmployeeID == employeeProfile.EmployeeID);
                
                if (existingEmployee != null)
                    return BadRequest(new { message = $"Employee with ID '{employeeProfile.EmployeeID}' already exists." });

                _context.EmployeeProfiles.Add(employeeProfile);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmployeeProfile), new { id = employeeProfile.EmployeeID }, employeeProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating employee profile", error = ex.Message });
            }
        }

        // PUT: api/EmployeeProfiles/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployeeProfile(string id, EmployeeProfile employeeProfile)
        {
            try
            {
                if (id != employeeProfile.EmployeeID)
                    return BadRequest(new { message = "Employee ID mismatch." });

                var existingEmployee = await _context.EmployeeProfiles.FindAsync(id);
                if (existingEmployee == null)
                    return NotFound(new { message = $"Employee with ID '{id}' not found." });

                _context.Entry(existingEmployee).CurrentValues.SetValues(employeeProfile);
                _context.Entry(existingEmployee).State = EntityState.Modified;
                
                await _context.SaveChangesAsync();
                return Ok(new { message = "Employee profile updated successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating employee profile", error = ex.Message });
            }
        }

        // DELETE: api/EmployeeProfiles/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployeeProfile(string id)
        {
            try
            {
                var employee = await _context.EmployeeProfiles
                    .Include(e => e.ProfileSkills)
                    .Include(e => e.ProfileEducation)
                    .FirstOrDefaultAsync(e => e.EmployeeID == id);

                if (employee == null)
                    return NotFound(new { message = $"Employee with ID '{id}' not found." });

                if (!string.IsNullOrEmpty(employee.ProfileImage))
                {
                    var imagePath = Path.Combine(_environment.ContentRootPath, employee.ProfileImage.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.EmployeeProfiles.Remove(employee);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Employee profile deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting employee profile", error = ex.Message });
            }
        }

        // POST: api/EmployeeProfiles/{id}/upload-photo
        [HttpPost("{id}/upload-photo")]
        public async Task<ActionResult> UploadProfilePhoto(string id, IFormFile photo)
        {
            try
            {
                var employee = await _context.EmployeeProfiles.FindAsync(id);
                if (employee == null)
                    return NotFound(new { message = $"Employee with ID '{id}' not found." });

                if (photo == null || photo.Length == 0)
                    return BadRequest(new { message = "No file uploaded." });

                var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
                if (!allowedTypes.Contains(photo.ContentType.ToLower()))
                    return BadRequest(new { message = "Invalid file type. Only JPEG, PNG, GIF, and WebP are allowed." });

                if (photo.Length > 5 * 1024 * 1024)
                    return BadRequest(new { message = "File size cannot exceed 5MB." });

                var uploadsFolder = Path.Combine(_environment.ContentRootPath, "uploads", "profiles");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                if (!string.IsNullOrEmpty(employee.ProfileImage))
                {
                    var oldImagePath = Path.Combine(_environment.ContentRootPath, employee.ProfileImage.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var fileExtension = Path.GetExtension(photo.FileName).ToLower();
                var fileName = $"{id}_{DateTime.Now:yyyyMMddHHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                var relativeImagePath = $"uploads/profiles/{fileName}";
                employee.ProfileImage = relativeImagePath;
                
                _context.Entry(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { 
                    message = "Profile photo uploaded successfully.", 
                    profileImageUrl = relativeImagePath,
                    fileName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading profile photo", error = ex.Message });
            }
        }

        // ... (Your other methods remain the same) ...
        // DELETE: api/EmployeeProfiles/{id}/delete-photo
        [HttpDelete("{id}/delete-photo")]
        public async Task<ActionResult> DeleteProfilePhoto(string id)
        {
            try
            {
                var employee = await _context.EmployeeProfiles.FindAsync(id);
                if (employee == null)
                    return NotFound(new { message = $"Employee with ID '{id}' not found." });

                if (string.IsNullOrEmpty(employee.ProfileImage))
                    return BadRequest(new { message = "No profile image to delete." });

                var imagePath = Path.Combine(_environment.ContentRootPath, employee.ProfileImage.TrimStart('/'));
                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                employee.ProfileImage = null;
                _context.Entry(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Profile photo deleted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting profile photo", error = ex.Message });
            }
        }

        // GET: api/EmployeeProfiles/by-user-id/{userId}
        [HttpGet("by-user-id/{userId}")]
        public async Task<ActionResult<EmployeeProfile>> GetEmployeeProfileByUserId(int userId)
        {
            try
            {
                var employee = await _context.EmployeeProfiles
                    .Include(e => e.ProfileSkills)
                    .Include(e => e.ProfileEducation)
                    .FirstOrDefaultAsync(e => e.UserID == userId);
                if (employee == null)
                    return NotFound(new { message = $"No employee profile found for user ID {userId}" });
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee profile by user ID", error = ex.Message });
            }
        }

        // GET: api/EmployeeProfiles/by-email/{email}
        [HttpGet("by-email/{email}")]
        public async Task<ActionResult<EmployeeProfile>> GetEmployeeProfileByEmail(string email)
        {
            try
            {
                var employee = await _context.EmployeeProfiles
                    .Include(e => e.ProfileSkills)
                    .Include(e => e.ProfileEducation)
                    .FirstOrDefaultAsync(e => e.Email == email);
                if (employee == null)
                    return NotFound(new { message = $"No employee profile found for email {email}" });
                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving employee profile by email", error = ex.Message });
            }
        }

        // GET: api/EmployeeProfiles/search
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<EmployeeProfile>>> SearchEmployeeProfiles(
            [FromQuery] string name = null,
            [FromQuery] string department = null,
            [FromQuery] string designation = null,
            [FromQuery] string email = null)
        {
            try
            {
                var query = _context.EmployeeProfiles
                    .Include(e => e.ProfileSkills)
                    .Include(e => e.ProfileEducation)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(name))
                    query = query.Where(e => e.Name.Contains(name));
                if (!string.IsNullOrEmpty(department))
                    query = query.Where(e => e.Department.Contains(department));
                if (!string.IsNullOrEmpty(designation))
                    query = query.Where(e => e.Designation.Contains(designation));
                if (!string.IsNullOrEmpty(email))
                    query = query.Where(e => e.Email.Contains(email));

                var employees = await query.ToListAsync();
                return Ok(employees);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error searching employee profiles", error = ex.Message });
            }
        }
    }
}