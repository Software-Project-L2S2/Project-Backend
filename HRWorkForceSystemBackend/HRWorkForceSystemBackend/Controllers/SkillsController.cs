using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models;
using HRWorkForceSystemBackend.DTOs.SkillDTOs;
using System.Threading.Tasks;
using HRWorkForceSystemBackend.Models.SkillsModels;

namespace HRWorkForceSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public SkillsController(AppDbContext context) { _context = context; }

        [HttpPost]
        public async Task<ActionResult<Skill>> PostSkill(SkillDto skillDto)
        {
            if (!await _context.Employees.AnyAsync(e => e.EmployeeID == skillDto.EmployeeID))
                return NotFound($"Employee with ID {skillDto.EmployeeID} not found.");

            var skill = new Skill
            {
                EmployeeID = skillDto.EmployeeID,
                SkillName = skillDto.SkillName,
                Description = skillDto.Description,
                Level = skillDto.Level
            };

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostSkill), new { id = skill.SkillID }, skill);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkill(int id, SkillDto skillDto)
        {
            var skillToUpdate = await _context.Skills.FindAsync(id);
            if (skillToUpdate == null) return NotFound();

            skillToUpdate.SkillName = skillDto.SkillName;
            skillToUpdate.Description = skillDto.Description;
            skillToUpdate.Level = skillDto.Level;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null) return NotFound();
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}