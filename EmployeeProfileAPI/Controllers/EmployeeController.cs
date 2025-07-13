using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using EmployeeProfileAPI.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;

namespace EmployeeProfileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public EmployeeController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: api/Employee
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
        {
            return await _context.Employees
                                 .Include(e => e.Skills)
                                 .Include(e => e.Education)
                                 .ToListAsync();
        }

        // GET: api/Employee/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmployee(int id)
        {
            var employee = await _context.Employees
                                         .Include(e => e.Skills)
                                         .Include(e => e.Education)
                                         .FirstOrDefaultAsync(e => e.EmployeeID == id);
            if (employee == null) 
            { 
                return NotFound(); 
            }
            return Ok(employee);
        }

        // PUT: api/Employee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployee(int id, [FromBody] UpdateEmployeeDto employeeDto)
        {
            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            // Update properties from the DTO
            existingEmployee.Name = employeeDto.Name;
            existingEmployee.Designation = employeeDto.Designation;
            existingEmployee.Department = employeeDto.Department;
            existingEmployee.Gender = employeeDto.Gender;
            existingEmployee.StartDate = employeeDto.StartDate;
            existingEmployee.Age = employeeDto.Age;
            existingEmployee.Contact = employeeDto.Contact;
            existingEmployee.Email = employeeDto.Email;
            existingEmployee.CompanyLogo = employeeDto.CompanyLogo;
            
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // *** THIS IS THE NEW METHOD THAT FIXES THE 405 ERROR ***
        // POST: api/Employee
        // This method handles the creation of a new employee resource.
        [HttpPost]
        public async Task<ActionResult<Employee>> PostEmployee([FromBody] Employee employee)
        {
            // The [FromBody] attribute tells ASP.NET Core to get the employee data 
            // from the body of the HTTP POST request.
            if (!ModelState.IsValid)
            {
                // If the incoming data doesn't match the model's requirements, return a bad request.
                return BadRequest(ModelState);
            }

            // Add the new employee object to the database context.
            _context.Employees.Add(employee);
            
            // Save the changes to the database.
            await _context.SaveChangesAsync();

            // Return a "201 Created" response. This is the standard for a successful POST that creates a new resource.
            // 'CreatedAtAction' also adds a 'Location' header to the response, pointing to the newly created employee's URL.
            return CreatedAtAction(nameof(GetEmployee), new { id = employee.EmployeeID }, employee);
        }

        // POST: api/Employee/5/upload-image
        [HttpPost("{id}/upload-image")]
        public async Task<IActionResult> UploadProfileImage(int id, [FromForm] IFormFile file)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }
            
            string wwwRootPath = _hostEnvironment.WebRootPath;
            string imageStoragePath = Path.Combine(wwwRootPath, "images");
            if (!Directory.Exists(imageStoragePath))
            {
                Directory.CreateDirectory(imageStoragePath);
            }

            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string finalFilePath = Path.Combine(imageStoragePath, fileName);

            using (var fileStream = new FileStream(finalFilePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            employee.ProfileImage = $"/images/{fileName}"; // Relative path
            
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();

            return Ok(new { profileImagePath = employee.ProfileImage });
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeID == id);
        }
    }
}