using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using EmployeeProfileAPI.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using Microsoft.AspNetCore.Http;

namespace EmployeeProfileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public EmployeeController(AppDbContext context, IWebHostEnvironment webHostEnvironment) 
        { 
            _context = context; 
            _hostingEnvironment = webHostEnvironment;
        }

        // GET: api/Employee/5 (Remains for direct ID access if needed)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees
                                         .Include(e => e.Skills)
                                         .Include(e => e.Education)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(e => e.EmployeeID == id);
            if (employee == null) 
            { 
                return NotFound(); 
            }
            return Ok(employee);
        }

        // NEW ENDPOINT: Get an employee profile by their email address.
        // GET: api/Employee/by-email/user@example.com
        [HttpGet("by-email/{email}")]
        public async Task<IActionResult> GetEmployeeByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest("Email address cannot be empty.");
            }

            var employee = await _context.Employees
                                         .Include(e => e.Skills)
                                         .Include(e => e.Education)
                                         .AsNoTracking()
                                         .FirstOrDefaultAsync(e => e.Email.ToLower() == email.ToLower());

            if (employee == null)
            {
                return NotFound($"Employee with email '{email}' not found.");
            }
            return Ok(employee);
        }

        // PUT: api/Employee/5 (Updated to use the DTO correctly)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] UpdateEmployeeDto employeeDto)
        {
            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            
            // Map DTO to the entity
            existingEmployee.Name = employeeDto.Name;
            existingEmployee.Designation = employeeDto.Designation;
            existingEmployee.Department = employeeDto.Department;
            existingEmployee.Gender = employeeDto.Gender;
            existingEmployee.StartDate = employeeDto.StartDate;
            existingEmployee.Age = employeeDto.Age;
            existingEmployee.Contact = employeeDto.Contact;
            existingEmployee.Email = employeeDto.Email;

            try
            {
                 await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                 if (!EmployeeExists(id)) { return NotFound(); }
                 else { throw; }
            }
           
            return NoContent();
        }
        
        // POST: api/Employee/{id}/upload-image
        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadImage(int id, [FromForm] IFormFile file)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound("Employee not found.");
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            var uploadsFolderPath = Path.Combine(_hostingEnvironment.WebRootPath, "images");
            if (!Directory.Exists(uploadsFolderPath)) Directory.CreateDirectory(uploadsFolderPath);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            employee.ProfileImage = $"/images/{uniqueFileName}";
            await _context.SaveChangesAsync();

            return Ok(new { profileImagePath = employee.ProfileImage });
        }

        // GET: api/Employee
[HttpGet]
public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
{
    var employees = await _context.Employees
                                  .Include(e => e.Skills)
                                  .Include(e => e.Education)
                                  .AsNoTracking()
                                  .ToListAsync();

    return Ok(employees);
}

        // DELETE: api/Employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees
                                         .Include(e => e.Skills)
                                         .Include(e => e.Education)
                                         .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            // Manually remove dependents if cascade delete is not configured
            _context.Skills.RemoveRange(employee.Skills);
            _context.Education.RemoveRange(employee.Education);
            _context.Employees.Remove(employee);
            
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Employee with ID {id} and related data deleted successfully." });
        }

        private bool EmployeeExists(int id) => _context.Employees.Any(e => e.EmployeeID == id);
    }
}