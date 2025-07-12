using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

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

        // ✅ GET: api/employee → Get all employees (for project page)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees()
        {
            var employees = await _context.Employees
                .Include(e => e.Skills)
                .Include(e => e.Education)
                .ToListAsync();

            return Ok(employees);
        }

        // ✅ GET: api/employee/{id} → Get one employee with skills & education
        [HttpGet("{id}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Skills)
                .Include(e => e.Education)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);

            if (employee == null)
                return NotFound(new { message = "Employee not found." });

            return Ok(employee);
        }

        // ✅ POST: api/employee/FindOrCreateFromUserId/{userId}
        [HttpPost("FindOrCreateFromUserId/{userId}")]
        public async Task<IActionResult> FindOrCreateEmployeeFromUserId(int userId)
        {
            var registeredUser = await _context.WorkforceUsers
                .Where(u => u.Id == userId)
                .Select(u => new { u.Email, u.FirstName, u.LastName, u.PhoneNumber })
                .FirstOrDefaultAsync();

            if (registeredUser == null)
            {
                return NotFound(new { message = "The registered user could not be found." });
            }

            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == registeredUser.Email);

            if (existingEmployee != null)
            {
                return Ok(new { employeeId = existingEmployee.EmployeeID });
            }

            var newEmployee = new Employee
            {
                Name = $"{registeredUser.FirstName} {registeredUser.LastName}",
                Email = registeredUser.Email,
                Contact = registeredUser.PhoneNumber,
                Designation = "Not Assigned",
                Department = "Not Assigned",
                Gender = "Not Specified",
                StartDate = System.DateTime.UtcNow,
                Age = 0,
                ProfileImage = "",
                CompanyLogo = ""
            };

            _context.Employees.Add(newEmployee);
            await _context.SaveChangesAsync();

            return Ok(new { employeeId = newEmployee.EmployeeID });
        }

        // ✅ PUT: api/employee/{id} → Update an employee profile
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] Employee employee)
        {
            if (id != employee.EmployeeID)
            {
                return BadRequest("ID mismatch.");
            }

            _context.Entry(employee).State = EntityState.Modified;

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

            return Ok(employee);
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.EmployeeID == id);
        }
    }
}
