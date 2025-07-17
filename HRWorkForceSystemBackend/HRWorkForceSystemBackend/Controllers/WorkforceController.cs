using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models.WorkforceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRWorkForceSystemBackend.DTOs.WorkforceDTOs;

namespace HRWorkForceSystemBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // Requires authentication for all endpoints
    public class WorkforceAnalyticsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        [HttpGet("movements")]
        public async Task<ActionResult<IEnumerable<MovementDto>>> GetMovements() // <-- Note the changed return type
        {
            var movements = await _context.Movements
                .Include(m => m.Employee) // This tells EF Core to also load the data from the related Employees table.
                .Select(m => new MovementDto // This transforms each result into the new DTO shape.
                {
                    Id = m.Id,
                    EmployeeId = m.EmployeeID,
                    Name = m.Employee.Name,         // Get the name from the Employee table
                    Department = m.Employee.Department, // Get the department from the Employee table
                    Status = m.MovementType,      // Rename 'MovementType' to 'Status' for the frontend
                    Date = m.EffectiveDate        // Rename 'EffectiveDate' to 'Date' for the frontend
                })
                .ToListAsync();

            return Ok(movements);
        }

        // GET: api/WorkforceAnalytics/attritions
        [HttpGet("attritions")]
        public async Task<ActionResult<IEnumerable<AttritionDto>>> GetAttritions() // <-- Note the changed return type
        {
            var attritions = await _context.Attritions
                .Include(a => a.Employee) // Load the related Employee data
                .Select(a => new AttritionDto // Project into the DTO
                {
                    Id = a.Id,
                    EmployeeId = a.EmployeeID,
                    Name = a.Employee.Name,
                    Department = a.Employee.Department,
                    Position = a.Employee.Designation, // Let's assume Designation is the Position
                    ExitDate = a.AttritionDate,
                    Notes = a.Details
                })
                .ToListAsync();

            return Ok(attritions);
        }

        // GET: api/WorkforceAnalytics/summary
        [HttpGet("summary")]
        public async Task<ActionResult<Summary>> GetSummary()
        {


            return new Summary
            {
                TotalPromotions = await _context.Movements.CountAsync(m => m.MovementType == "Promotion"),
                TotalExits = await _context.Movements.CountAsync(m => m.MovementType == "Exit"),
                TotalTransfers = await _context.Movements.CountAsync(m => m.MovementType == "Transfer"),
                TotalAttritions = await _context.Attritions.CountAsync(),
                TotalEmployees = await _context.Employees.CountAsync()

            };
        }

        // POST: api/WorkforceAnalytics/movement
        [HttpPost("movement")]
        public async Task<ActionResult<Movement>> CreateMovement([FromBody] CreateMovementDto dto) // <-- Use the DTO
        {
            // Manually create the Movement model from the DTO
            var movement = new Movement
            {
                EmployeeID = dto.EmployeeID,
                MovementType = dto.MovementType,
                NewPosition = dto.NewPosition,
                NewDepartment = dto.NewDepartment,
                Description = dto.Description,
                EffectiveDate = dto.EffectiveDate
                // We do NOT set the Employee navigation property here
            };

            _context.Movements.Add(movement);
            await _context.SaveChangesAsync();

            // It's good practice to return the created object, but we'll keep it simple
            return Ok(movement);
        }

        // POST: api/WorkforceAnalytics/attrition
        [HttpPost("attrition")]
        public async Task<ActionResult<Attrition>> CreateAttrition([FromBody] CreateAttritionDto dto) // <-- Use the DTO
        {
            // Manually create the Attrition model from the DTO
            var attrition = new Attrition
            {
                EmployeeID = dto.EmployeeID,
                Reason = dto.Reason,
                Details = dto.Details,
                AttritionDate = dto.AttritionDate
            };

            _context.Attritions.Add(attrition);
            await _context.SaveChangesAsync();

            return Ok(attrition);
        }

        // DELETE: api/WorkforceAnalytics/movement/{id}
        [HttpDelete("movement/{id}")]
        public async Task<IActionResult> DeleteMovement(int id)
        {
            var movement = await _context.Movements.FindAsync(id);
            if (movement == null) return NotFound();

            _context.Movements.Remove(movement);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/WorkforceAnalytics/attrition/{id}
        [HttpDelete("attrition/{id}")]
        public async Task<IActionResult> DeleteAttrition(int id)
        {
            var attrition = await _context.Attritions.FindAsync(id);
            if (attrition == null) return NotFound();

            _context.Attritions.Remove(attrition);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        
        [HttpGet("employees-by-department")]
public async Task<ActionResult<IEnumerable<DepartmentCountDto>>> GetEmployeesByDepartment()
{
    var departmentCounts = await _context.Employees
        .GroupBy(e => e.Department) // Group all employees by their department name
        .Select(group => new DepartmentCountDto
        {
            Department = group.Key, // The Key is the department name (e.g., "IT", "HR")
            Count = group.Count()   // Count how many employees are in that group
        })
        .OrderBy(d => d.Department) // Order alphabetically for a consistent look
        .ToListAsync();

    return Ok(departmentCounts);
}
    }
}