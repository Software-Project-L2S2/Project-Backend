using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.DTOs.TrainingProgramDTOs;
using HRWorkForceSystemBackend.Models.TrainingProgramModels;
using HRWorkForceSystemBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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


        [Authorize(Roles = "HR")]
        [HttpPost("training-program")]
        public async Task<IActionResult> CreateTrainingProgram([FromBody] TrainingProgramDto dto)
        {
            var program = new TrainingProgram
            {
                Name = dto.Name,
                Description = dto.Description,
                Availability = dto.Availability
            };

            _context.TrainingPrograms.Add(program);
            await _context.SaveChangesAsync();

            return Ok("Training program created.");
        }


        [Authorize(Roles = "HR")]
        [HttpPut("training-program/{id}")]
        public async Task<IActionResult> UpdateTrainingProgram(int id, [FromBody] TrainingProgramDto dto)
        {
            var program = await _context.TrainingPrograms.FindAsync(id);
            if (program == null) return NotFound("Training program not found.");

            program.Name = dto.Name;
            program.Description = dto.Description;
            program.Availability = dto.Availability;

            await _context.SaveChangesAsync();
            return Ok("Training program updated.");
        }


        [Authorize(Roles = "HR")]
        [HttpDelete("training-program/{id}")]
        public async Task<IActionResult> DeleteTrainingProgram(int id)
        {
            var program = await _context.TrainingPrograms.FindAsync(id);
            if (program == null) return NotFound("Training program not found.");

            _context.TrainingPrograms.Remove(program);
            await _context.SaveChangesAsync();
            return Ok("Training program deleted.");
        }


        [HttpGet("training-programs")]
        public async Task<IActionResult> GetAllAvailablePrograms()
        {
            var programs = await _context.TrainingPrograms
                .Where(tp => tp.Availability > 0)
                .ToListAsync();

            return Ok(programs);
        }

        [Authorize(Roles = "Workforce")]
        [HttpPost("enroll")]
        public async Task<IActionResult> Enroll([FromBody] EnrollmentDto dto)
        {
            var program = await _context.TrainingPrograms.FindAsync(dto.CourseId);

            if (program == null)
                return NotFound("Training program not found.");

            if (program.Availability <= 0)
                return BadRequest("No spots available in this program.");

            
            var existingEnrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.Email == dto.Email && e.TrainingProgramId == dto.CourseId);

            if (existingEnrollment != null)
                return BadRequest("You have already enrolled in this training program.");

           
            var enrollment = new Enrollment
            {
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                TrainingProgramId = dto.CourseId
            };

            
            program.Availability--;

            
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            
            var subject = $"Enrollment Confirmation - {program.Name}";
            var message = $@"
        <p>Dear {dto.FullName},</p>
        <p>Congratulations! You have successfully enrolled in the training program:</p>
        <ul>
            <li><strong>Program:</strong> {program.Name}</li>
            <li><strong>Description:</strong> {program.Description}</li>
        </ul>
        <p>We look forward to your participation.</p>
        <p>Best regards,<br/>HR Workforce System Team</p>
    ";

            try
            {
                await _emailService.SendEmailAsync(dto.Email, subject, message);
            }
            catch (Exception ex)
            {
                
                Console.WriteLine("Email send failed: " + ex.Message);
               
                return Ok("Enrolled successfully. However, we were unable to send a confirmation email.");
            }

            return Ok("Enrolled successfully. A confirmation email has been sent.");
        }
 

    }
}
