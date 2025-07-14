using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using EmployeeProfileAPI.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeProfileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context) 
        { 
            _context = context; 
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
        public async Task<IActionResult> PutEmployee(int id, UpdateEmployeeDto employeeDto)
        {
            var existingEmployee = await _context.Employees.FindAsync(id);
            if (existingEmployee == null)
            {
                return NotFound($"Employee with ID {id} not found.");
            }

            // Map from the DTO to the database entity
            existingEmployee.Name = employeeDto.Name;
            existingEmployee.Designation = employeeDto.Designation;
            existingEmployee.Department = employeeDto.Department;
            existingEmployee.Gender = employeeDto.Gender;
            existingEmployee.StartDate = employeeDto.StartDate;
            existingEmployee.Age = employeeDto.Age;
            existingEmployee.Contact = employeeDto.Contact;
            existingEmployee.Email = employeeDto.Email;
            existingEmployee.ProfileImage = employeeDto.ProfileImage;
            existingEmployee.CompanyLogo = employeeDto.CompanyLogo;
            
            await _context.SaveChangesAsync();
            return NoContent();
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

            // Remove related Skills and Education first to avoid FK constraint issues
            _context.Skills.RemoveRange(employee.Skills);
            _context.Education.RemoveRange(employee.Education);

            _context.Employees.Remove(employee);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, $"Error deleting employee: {ex.Message}");
            }

            return Ok(new { message = $"Employee with ID {id} and related data deleted successfully." });
        }

        private bool EmployeeExists(int id) => _context.Employees.Any(e => e.EmployeeID == id);
    }
}
