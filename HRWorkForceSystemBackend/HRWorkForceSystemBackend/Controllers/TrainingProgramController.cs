using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.DTOs.TrainingProgramDTOs;
using HRWorkForceSystemBackend.Models;
using HRWorkForceSystemBackend.Models.TrainingProgramModels;
using HRWorkForceSystemBackend.Models.SkillsModels;
using HRWorkForceSystemBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HRWorkForceSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingProgramController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EmailService _emailService;

        public TrainingProgramController(AppDbContext context, EmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // =================================================================
        // == Endpoints for HR and Admin Roles
        // =================================================================

        [HttpPost("create-assigned-program")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> CreateAssignedTrainingProgram([FromBody] CreateTrainingProgramDto dto)
        {
            var program = new TrainingProgram
            {
                Name = dto.Name,
                Description = dto.Description,
                TargetSkill = dto.TargetSkill,
                RequiredProficiencyLevel = dto.RequiredProficiencyLevel,
                Mode = dto.Mode,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                ResourceLink = dto.ResourceLink,
                TrainerDetails = dto.TrainerDetails,
                AutoAssignment = dto.AutoAssignment,
                Availability = 0
            };

            _context.TrainingPrograms.Add(program);
            await _context.SaveChangesAsync();

            if (program.AutoAssignment)
            {
                await AutoAssignEmployees(program);
            }

            return Ok(new { Message = "Assigned training program created successfully.", ProgramId = program.Id });
        }

        [HttpPost("manual-assign")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> ManualAssignEmployees([FromBody] ManualAssignmentDto dto)
        {
            var program = await _context.TrainingPrograms.FindAsync(dto.TrainingProgramId);
            if (program == null) return NotFound("Training program not found.");

            foreach (var employeeId in dto.EmployeeIds)
            {
                if (!await _context.Employees.AnyAsync(e => e.EmployeeID == employeeId)) continue;
                if (await _context.TrainingAssignments.AnyAsync(a => a.EmployeeID == employeeId && a.TrainingProgramId == dto.TrainingProgramId)) continue;

                var assignment = new TrainingAssignment { EmployeeID = employeeId, TrainingProgramId = dto.TrainingProgramId, Status = "Assigned" };
                _context.TrainingAssignments.Add(assignment);
            }

            await _context.SaveChangesAsync();
            return Ok("Employees assigned successfully.");
        }

        [HttpGet("dashboard")]
        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> GetTrainingDashboard()
        {
            var programs = await _context.TrainingPrograms
                .Include(p => p.Assignments)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.TargetSkill,
                    p.RequiredProficiencyLevel,
                    p.StartDate,
                    p.EndDate,
                    TotalAssigned = p.Assignments.Count(),
                    CompletedCount = p.Assignments.Count(a => a.Status == "Completed")
                })
                .ToListAsync();
            return Ok(programs);
        }

        // =================================================================
        // == Endpoints for Workforce (Self-Enrollment)
        // =================================================================

        [HttpGet("View-TrainingPrograms")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAvailablePrograms([FromQuery] string search)
        {
            var query = _context.TrainingPrograms.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(tp => tp.Name.Contains(search));

            var programs = await query.Where(tp => tp.Availability > 0).ToListAsync();
            return Ok(programs);
        }

        [HttpPost("enroll")]
        [Authorize(Roles = "Workforce")]
        public async Task<IActionResult> Enroll([FromBody] EnrollmentDto dto)
        {
            var program = await _context.TrainingPrograms.FindAsync(dto.CourseId);
            if (program == null) return NotFound("Training program not found.");
            if (program.Availability <= 0) return BadRequest("No spots available in this program.");

            var existingEnrollment = await _context.Enrollments.FirstOrDefaultAsync(e => e.Email == dto.Email && e.TrainingProgramId == dto.CourseId);
            if (existingEnrollment != null) return BadRequest("You have already enrolled in this training program.");

            var enrollment = new Enrollment
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                TrainingProgramId = dto.CourseId,
            };

            program.Availability--;
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var subject = $"Enrollment Confirmation - {program.Name}";
            var message = $"<p>Dear {dto.FullName},</p><p>Congratulations! You have successfully enrolled in the training program: {program.Name}.</p>";

            try { await _emailService.SendEmailAsync(dto.Email, subject, message); }
            catch (Exception ex)
            {
                Console.WriteLine("Email send failed: " + ex.Message);
                return Ok("Enrolled successfully. However, we were unable to send a confirmation email.");
            }

            return Ok("Enrolled successfully. A confirmation email has been sent.");
        }

        // =================================================================
        // == Endpoints for a specific Employee (Workforce Role)
        // =================================================================

        [HttpGet("my-dashboard")]
        [Authorize(Roles = "Workforce")]
        public async Task<IActionResult> GetMyDashboard()
        {
            var employeeIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(employeeIdStr, out var employeeId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var assignments = await _context.TrainingAssignments
                .Where(a => a.EmployeeID == employeeId)
                .Include(a => a.TrainingProgram)
                .ToListAsync();

            var completedCount = assignments.Count(a => a.Status == "Completed");
            var ongoingCount = assignments.Count(a => a.Status == "In Progress" || (a.Status == "Assigned" && a.TrainingProgram.StartDate <= DateTime.UtcNow && a.TrainingProgram.EndDate >= DateTime.UtcNow));
            var upcomingCount = assignments.Count(a => a.Status == "Assigned" && a.TrainingProgram.StartDate > DateTime.UtcNow);

            var skillsInProgress = assignments
                .Where(a => a.Status != "Completed")
                .Select(a => a.TrainingProgram.TargetSkill)
                .Distinct()
                .ToList();

            return Ok(new
            {
                completedCount,
                ongoingCount,
                upcomingCount,
                skillsInProgress
            });
        }

        [HttpGet("my-programs")]
        [Authorize(Roles = "Workforce")]
        public async Task<IActionResult> GetMyPrograms()
        {
            var employeeIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(employeeIdStr, out var employeeId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var assignedPrograms = await _context.TrainingAssignments
                .Where(a => a.EmployeeID == employeeId)
                .Include(a => a.TrainingProgram)
                .Select(a => new MyProgramDto
                {
                    Id = a.TrainingProgramId,
                    Name = a.TrainingProgram.Name,
                    Description = a.TrainingProgram.Description,
                    TargetSkill = a.TrainingProgram.TargetSkill,
                    Status = a.Status,
                    StartDate = a.TrainingProgram.StartDate,
                    EndDate = a.TrainingProgram.EndDate,
                    Mode = a.TrainingProgram.Mode,
                    AssignmentType = "Mandatory"
                })
                .ToListAsync();

            return Ok(assignedPrograms);
        }

        // Private helper method for auto-assignment
        private async Task AutoAssignEmployees(TrainingProgram program)
        {
            var employeesToAssign = await _context.Employees
                .Include(e => e.Skills)
                .Where(e => e.Skills.Any(s => s.SkillName == program.TargetSkill && s.Level < program.RequiredProficiencyLevel))
                .ToListAsync();

            foreach (var employee in employeesToAssign)
            {
                if (!await _context.TrainingAssignments.AnyAsync(a => a.EmployeeID == employee.EmployeeID && a.TrainingProgramId == program.Id))
                {
                    var assignment = new TrainingAssignment { EmployeeID = employee.EmployeeID, TrainingProgramId = program.Id, Status = "Assigned" };
                    _context.TrainingAssignments.Add(assignment);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}