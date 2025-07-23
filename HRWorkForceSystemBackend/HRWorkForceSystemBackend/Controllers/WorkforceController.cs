// Make sure this using directive correctly points to the location of your DTO file.
using HRWorkForceSystemBackend.DTOs.WorkforceDTOs; 
using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models.WorkforceModels; // Assuming your user models are here
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRWorkForceSystemBackend.Controllers
{
    [ApiController]
    [Route("api/WorkforceAnalytics")]
    public class WorkforceAnalyticsController(AppDbContext context) : ControllerBase
    {
        private readonly AppDbContext _context = context;

        // GET: api/WorkforceAnalytics/movements (No changes)
        [HttpGet("movements")]
        public async Task<ActionResult<IEnumerable<MovementDto>>> GetMovements()
        {
            var movements = await _context.Movements
                .Include(m => m.Employee)
                .Select(m => new MovementDto
                {
                    Id = m.Id,
                    EmployeeId = m.EmployeeID,
                    Name = m.Employee != null ? m.Employee.Name : "Former Employee",
                    Department = m.NewDepartment,
                    Status = m.MovementType,
                    Date = m.EffectiveDate,
                    NewPosition = m.NewPosition,
                    Description = m.Description
                })
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            return Ok(movements);
        }

        // GET: api/WorkforceAnalytics/attritions (No changes)
        [HttpGet("attritions")]
        public async Task<ActionResult<IEnumerable<AttritionDto>>> GetAttritions()
        {
            var attritions = await _context.Attritions
                .Select(a => new AttritionDto
                {
                    Id = a.Id,
                    EmployeeId = a.EmployeeID,
                    Name = a.Employee != null ? a.Employee.Name : $"Former Employee (ID: {a.EmployeeID})",
                    Department = a.Employee != null ? a.Employee.Department : "N/A",
                    Position = a.Employee != null ? a.Employee.Designation : "N/A",
                    ExitDate = a.AttritionDate,
                    Reason = a.Reason,
                    Notes = a.Details
                })
                .OrderByDescending(a => a.ExitDate)
                .ToListAsync();

            return Ok(attritions);
        }

        // GET: api/WorkforceAnalytics/summary (No changes)
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
        
        // GET: api/WorkforceAnalytics/employees-by-department (No changes)
        [HttpGet("employees-by-department")]
        public async Task<ActionResult<IEnumerable<DepartmentCountDto>>> GetEmployeesByDepartment()
        {
            var departmentCounts = await _context.Employees
                .Where(e => e.Department != null)
                .GroupBy(e => e.Department)
                .Select(group => new DepartmentCountDto
                {
                    Department = group.Key,
                    Count = group.Count()
                })
                .OrderBy(d => d.Department)
                .ToListAsync();

            return Ok(departmentCounts);
        }

        // POST: api/WorkforceAnalytics/movement (No changes)
        [HttpPost("movement")]
        public async Task<ActionResult<Movement>> CreateMovement([FromBody] CreateMovementDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var employee = await _context.Employees.FindAsync(dto.EmployeeID);
                if (employee == null)
                {
                    return NotFound(new { message = $"Employee with ID {dto.EmployeeID} not found." });
                }

                var movement = new Movement
                {
                    EmployeeID = dto.EmployeeID,
                    MovementType = dto.MovementType,
                    NewPosition = dto.NewPosition,
                    NewDepartment = dto.NewDepartment,
                    Description = dto.Description,
                    EffectiveDate = dto.EffectiveDate
                };
                _context.Movements.Add(movement);

                if (dto.MovementType == "Promotion" || dto.MovementType == "Transfer")
                {
                    if (!string.IsNullOrEmpty(dto.NewPosition)) employee.Designation = dto.NewPosition;
                    if (!string.IsNullOrEmpty(dto.NewDepartment)) employee.Department = dto.NewDepartment;
                    _context.Employees.Update(employee);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(movement);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { message = $"Internal server error: {ex.Message}" });
            }
        }

        // POST: api/WorkforceAnalytics/attrition (*** UPDATED AND INTEGRATED METHOD ***)
        [HttpPost("attrition")]
        public async Task<ActionResult<Attrition>> CreateAttrition([FromBody] CreateAttritionDto dto)
        {
            // Use a transaction to ensure all database operations succeed or fail together.
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // The ID from the DTO will be treated as a generic User ID, not just an Employee ID.
                var userId = dto.EmployeeID;
                string userEmail = null; // We need the email to find the user in other tables.
                bool userFound = false;

                // --- Step 1: Find the user in ANY role table and mark them for deletion ---
                
                // Check the Employees table
                var employee = await _context.Employees.FindAsync(userId);
                if (employee != null)
                {
                    userEmail = employee.Email;
                    _context.Employees.Remove(employee);
                    userFound = true;
                }

                // If not an employee, check the Admins table
                if (!userFound)
                {
                    // Assuming you have a context set for Admins like `_context.Admins`
                    var admin = await _context.Admins.FindAsync(userId);
                    if (admin != null)
                    {
                        userEmail = admin.Email;
                        _context.Admins.Remove(admin);
                        userFound = true;
                    }
                }
                
                // If not an employee or admin, check the HRUsers table
                if (!userFound)
                {
                    // Assuming you have a context set for HRUsers like `_context.HRUsers`
                    var hrUser = await _context.HRUsers.FindAsync(userId);
                    if (hrUser != null)
                    {
                        userEmail = hrUser.Email;
                        _context.HRUsers.Remove(hrUser);
                        userFound = true;
                    }
                }

                // If the user ID was not found in any role table, return an error.
                if (!userFound)
                {
                    return NotFound(new { message = $"User with ID {userId} not found in any role (Employee, Admin, HR)." });
                }

                // --- Step 2: Remove the user from the central login table (`WorkforceUsers`) ---
                if (!string.IsNullOrEmpty(userEmail))
                {
                    var workforceUser = await _context.WorkforceUsers.FirstOrDefaultAsync(u => u.Email == userEmail);
                    if (workforceUser != null)
                    {
                        _context.WorkforceUsers.Remove(workforceUser);
                    }
                }

                // --- Step 3: Create the official Attrition record ---
                var attrition = new Attrition
                {
                    // IMPORTANT: We use the original ID, even if it's from an Admin or HR user.
                    // This assumes your Attrition table's EmployeeID can store these IDs.
                    // If a strict foreign key to `dbo.Employees` exists, this will fail for non-employees.
                    EmployeeID = userId, 
                    Reason = dto.Reason,
                    Details = dto.Details,
                    AttritionDate = dto.AttritionDate
                };
                _context.Attritions.Add(attrition);
                
                // --- Step 4: Save all changes to the database ---
                // This single command will execute all the .Remove() and .Add() operations.
                await _context.SaveChangesAsync();

                // If everything was successful, commit the transaction.
                await transaction.CommitAsync();
                
                return Ok(attrition);
            }
            catch (Exception ex)
            {
                // If any error occurs, roll back the entire transaction to prevent partial data changes.
                await transaction.RollbackAsync();
                // Provide a more detailed error message for debugging.
                return StatusCode(500, new { message = $"An error occurred during the attrition process: {ex.Message}" });
            }
        }
        
        // PUT: api/WorkforceAnalytics/movement/{id} (No changes)
        [HttpPut("movement/{id}")]
        public async Task<IActionResult> UpdateMovement(int id, [FromBody] MovementDto dto)
        {
            var movement = await _context.Movements.FindAsync(id);
            if (movement == null) return NotFound();

            movement.NewPosition = dto.NewPosition;
            movement.NewDepartment = dto.Department;
            movement.Description = dto.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // PUT: api/WorkforceAnalytics/attrition/{id} (No changes)
        [HttpPut("attrition/{id}")]
        public async Task<IActionResult> UpdateAttrition(int id, [FromBody] AttritionDto dto)
        {
            var attrition = await _context.Attritions.FindAsync(id);
            if (attrition == null) return NotFound();
            
            attrition.Reason = dto.Reason;
            attrition.Details = dto.Notes;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/WorkforceAnalytics/movement/{id} (No changes)
        [HttpDelete("movement/{id}")]
        public async Task<IActionResult> DeleteMovement(int id)
        {
            var movement = await _context.Movements.FindAsync(id);
            if (movement == null) return NotFound();

            _context.Movements.Remove(movement);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/WorkforceAnalytics/attrition/{id} (No changes)
        [HttpDelete("attrition/{id}")]
        public async Task<IActionResult> DeleteAttrition(int id)
        {
            var attrition = await _context.Attritions.FindAsync(id);
            if (attrition == null) return NotFound();

            _context.Attritions.Remove(attrition);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}