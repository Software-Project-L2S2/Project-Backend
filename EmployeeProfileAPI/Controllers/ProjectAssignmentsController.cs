using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data; // Assuming this is your DbContext's namespace
using EmployeeProfileAPI.Models; // Assuming this is your Models' namespace
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient; // Required for SqlException

namespace EmployeeProfileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProjectAssignmentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProjectAssignmentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/ProjectAssignments
        [HttpGet]
        public async Task<IActionResult> GetProjectAssignments()
        {
            try
            {
                // Ensure the primary key 'AssignmentID' is selected
                var assignments = await _context.ProjectAssignments
                    .Select(a => new
                    {
                        a.AssignmentID, // This is the primary key your frontend needs
                        a.ProjectID,
                        a.EmployeeID,
                        a.AssignedDate
                    })
                    .ToListAsync();

                return Ok(assignments);
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return StatusCode(500, "An internal server error occurred while retrieving assignments.");
            }
        }

        // GET: api/ProjectAssignments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectAssignmentById(int id)
        {
            try
            {
                var assignment = await _context.ProjectAssignments
                    .Select(a => new
                    {
                        a.AssignmentID,
                        a.ProjectID,
                        a.EmployeeID,
                        a.AssignedDate
                    })
                    .FirstOrDefaultAsync(a => a.AssignmentID == id);

                if (assignment == null)
                {
                    return NotFound("Assignment not found.");
                }

                return Ok(assignment);
            }
            catch (Exception ex)
            {
                // Log the exception ex
                return StatusCode(500, "An internal server error occurred.");
            }
        }

        // POST: api/ProjectAssignments
        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] ProjectAssignmentDto assignmentDto)
        {
            if (assignmentDto == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // --- Robust Validation ---
            var projectExists = await _context.Projects.AnyAsync(p => p.ProjectID == assignmentDto.ProjectID);
            if (!projectExists)
            {
                return NotFound($"Project with ID {assignmentDto.ProjectID} does not exist.");
            }

            var employeeExists = await _context.Employees.AnyAsync(e => e.EmployeeID == assignmentDto.EmployeeID);
            if (!employeeExists)
            {
                return NotFound($"Employee with ID {assignmentDto.EmployeeID} does not exist.");
            }

            var existingAssignment = await _context.ProjectAssignments
                .AnyAsync(a => a.ProjectID == assignmentDto.ProjectID && a.EmployeeID == assignmentDto.EmployeeID);
            if (existingAssignment)
            {
                return Conflict("This employee is already assigned to this project.");
            }

            var project = await _context.Projects.FindAsync(assignmentDto.ProjectID);
            var currentAssignmentCount = await _context.ProjectAssignments.CountAsync(a => a.ProjectID == assignmentDto.ProjectID);
            if (currentAssignmentCount >= project.EmployeeCount)
            {
                return Conflict("Project has reached its maximum employee capacity.");
            }
            // --- End Validation ---

            var assignment = new ProjectAssignment
            {
                ProjectID = assignmentDto.ProjectID,
                EmployeeID = assignmentDto.EmployeeID,
                AssignedDate = DateTime.UtcNow // Always use UTC for server time
            };

            _context.ProjectAssignments.Add(assignment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Handle potential database-level errors (e.g., foreign key violations if checks fail)
                if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 547))
                {
                    return BadRequest("The operation failed due to a foreign key constraint. The specified Project or Employee may no longer exist.");
                }
                // Log the exception ex
                return StatusCode(500, "A database error occurred while saving the assignment.");
            }
            catch(Exception ex)
            {
                 // Log the exception ex
                return StatusCode(500, "An unexpected error occurred.");
            }

            return CreatedAtAction(
                nameof(GetProjectAssignmentById),
                new { id = assignment.AssignmentID },
                assignment // Return the newly created object
            );
        }

        // ✅ NEW - DELETE: api/ProjectAssignments/5
        // This is the method that was missing and caused the 405 error.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProjectAssignment(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid assignment ID.");
            }

            var projectAssignment = await _context.ProjectAssignments.FindAsync(id);

            if (projectAssignment == null)
            {
                // It's okay if it's already gone. The client's goal is achieved.
                // Alternatively, return NotFound() if you want to be strict.
                return NoContent();
            }

            _context.ProjectAssignments.Remove(projectAssignment);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Handle cases where deletion is blocked by database constraints
                 // Log the exception ex
                return StatusCode(500, $"A database error occurred while deleting the assignment: {ex.Message}");
            }
            catch(Exception ex)
            {
                 // Log the exception ex
                return StatusCode(500, "An unexpected error occurred.");
            }

            // A 204 No Content is the standard and correct response for a successful DELETE.
            return NoContent();
        }
    }

    // DTO for creating a ProjectAssignment. This defines the data sent from the frontend.
    // Your existing DTO is correct.
    public class ProjectAssignmentDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be a positive number.")]
        public int ProjectID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeID must be a positive number.")]
        public int EmployeeID { get; set; }
    }
}