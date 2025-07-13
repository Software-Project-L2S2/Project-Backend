using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.Data.SqlClient; // ADDED: Required for SqlException

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

        // GET methods are unchanged and correct. I'm omitting them for brevity.
        // ... (Your GET methods from the original file go here) ...
        #region Unchanged GET Methods
        [HttpGet]
        public async Task<IActionResult> GetProjectAssignments()
        {
            try
            {
                var assignments = await _context.ProjectAssignments
                    .Include(a => a.Project)
                    .Select(a => new { a.AssignmentID, a.ProjectID, a.EmployeeID, a.AssignedDate, Project = a.Project })
                    .ToListAsync();
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProjectAssignmentById(int id)
        {
            try
            {
                var assignment = await _context.ProjectAssignments
                    .Include(a => a.Project)
                    .Select(a => new { a.AssignmentID, a.ProjectID, a.EmployeeID, a.AssignedDate, Project = a.Project })
                    .FirstOrDefaultAsync(a => a.AssignmentID == id);
                if (assignment == null) return NotFound("Assignment not found.");
                return Ok(assignment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion

        // POST: api/ProjectAssignments
        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] ProjectAssignmentDto assignmentDto)
        {
            if (assignmentDto == null)
                return BadRequest("Invalid assignment data.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (assignmentDto.ProjectID <= 0 || assignmentDto.EmployeeID <= 0)
                return BadRequest("ProjectID and EmployeeID must be greater than 0.");
            
            // Your validation is good. It checks if entities exist before trying to assign.
            var project = await _context.Projects.FindAsync(assignmentDto.ProjectID);
            if (project == null)
                return NotFound($"Project with ID {assignmentDto.ProjectID} does not exist.");

            var employee = await _context.Employees.FindAsync(assignmentDto.EmployeeID);
            if (employee == null)
                return NotFound($"Employee with ID {assignmentDto.EmployeeID} does not exist.");

            var existingAssignment = await _context.ProjectAssignments
                .AnyAsync(a => a.ProjectID == assignmentDto.ProjectID && a.EmployeeID == assignmentDto.EmployeeID);
            if (existingAssignment)
                return Conflict("Employee is already assigned to this project.");

            var currentAssignments = await _context.ProjectAssignments
                .CountAsync(a => a.ProjectID == assignmentDto.ProjectID);
            if (currentAssignments >= project.EmployeeCount)
                return Conflict("Project has reached maximum employee capacity.");

            var assignment = new ProjectAssignment
            {
                ProjectID = assignmentDto.ProjectID,
                EmployeeID = assignmentDto.EmployeeID,
                AssignedDate = DateTime.UtcNow // Use UtcNow for consistency
            };

            _context.ProjectAssignments.Add(assignment);

            try
            {
                await _context.SaveChangesAsync();
            }
            // --- START OF CORRECTED ERROR HANDLING ---
            catch (DbUpdateException ex)
            {
                // Inspect the inner exception to find the root cause from the database.
                if (ex.InnerException is SqlException sqlEx)
                {
                    switch (sqlEx.Number)
                    {
                        case 547: // Foreign Key violation
                            return BadRequest("The operation failed because the specified Project or Employee does not exist.");
                        case 2627: // Unique Key violation
                        case 2601:
                            return Conflict("This operation violates a unique constraint. The item may already exist.");
                        default:
                            // For other SQL errors, return a generic database error.
                            return StatusCode(500, "A database error occurred while saving the assignment.");
                    }
                }
                // For non-SQL database errors
                return StatusCode(500, $"A database update error occurred: {ex.Message}");
            }
            catch (Exception ex)
            {
                // For any other unexpected errors
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
            // --- END OF CORRECTED ERROR HANDLING ---

            // To avoid another database trip, you can build the response object manually
            var createdAssignmentResponse = new 
            {
                assignment.AssignmentID,
                assignment.ProjectID,
                assignment.EmployeeID,
                assignment.AssignedDate,
                Project = new { project.ProjectID, project.Name, project.Status } // simplified project
            };

            return CreatedAtAction(
                nameof(GetProjectAssignmentById),
                new { id = assignment.AssignmentID },
                createdAssignmentResponse
            );
        }

        // It's good practice to apply the same robust error handling to your PUT/DELETE methods
        // ... (Your PUT and DELETE methods would go here, updated with the same try/catch logic) ...
    }

    // DTO for ProjectAssignment (This is correct)
    public class ProjectAssignmentDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ProjectID must be greater than 0")]
        public int ProjectID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "EmployeeID must be greater than 0")]
        public int EmployeeID { get; set; }

        public DateTime? AssignedDate { get; set; }
    }
}