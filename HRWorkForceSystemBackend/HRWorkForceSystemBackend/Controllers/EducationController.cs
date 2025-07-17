using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models;
using HRWorkForceSystemBackend.DTOs.SkillDTOs; // IMPORTANT: Add this line
using System.Threading.Tasks;
using HRWorkForceSystemBackend.Models.SkillsModels;


namespace HRWorkForceSystemBackend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EducationController : ControllerBase
	{
		private readonly AppDbContext _context;
		public EducationController(AppDbContext context) { _context = context; }

		[HttpPost]
		public async Task<ActionResult<Education>> PostEducation(EducationDto educationDto)
		{
			if (!await _context.Employees.AnyAsync(e => e.EmployeeID == educationDto.EmployeeID))
				return NotFound($"Employee with ID {educationDto.EmployeeID} not found.");

			var education = new Education
			{
				EmployeeID = educationDto.EmployeeID,
				Qualification = educationDto.Qualification
			};

			_context.Education.Add(education);
			await _context.SaveChangesAsync();
			return CreatedAtAction(nameof(PostEducation), new { id = education.EducationID }, education);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> PutEducation(int id, EducationDto educationDto)
		{
			var educationToUpdate = await _context.Education.FindAsync(id);
			if (educationToUpdate == null) return NotFound();

			educationToUpdate.Qualification = educationDto.Qualification;

			await _context.SaveChangesAsync();
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteEducation(int id)
		{
			var education = await _context.Education.FindAsync(id);
			if (education == null) return NotFound();
			_context.Education.Remove(education);
			await _context.SaveChangesAsync();
			return NoContent();
		}
	}
}