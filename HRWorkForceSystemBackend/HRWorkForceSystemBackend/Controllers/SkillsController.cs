// --- SkillsController.cs ---
// This code is complete and correct.

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models; // Ensure your base Skill model is here
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using HRWorkForceSystemBackend.Models.SkillsModels; // Ensure this namespace is correct
using HRWorkForceSystemBackend.DTOs.SkillDTOs;       // Ensure this namespace is correct

namespace HRWorkForceSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SkillsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Skills
        // This is the endpoint the frontend will call to get all skills.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Skill>>> GetSkills()
        {
            try
            {
                // This returns the full list of skills, including EmployeeID for each.
                return await _context.Skills.ToListAsync();
            }
            catch (Exception ex)
            {
                // Good practice to log the error for debugging.
                // Console.WriteLine(ex);
                return StatusCode(500, "An internal server error occurred while retrieving skills.");
            }
        }

        // GET: api/Skills/5
        // Gets a single skill by its primary key.
        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);

            if (skill == null)
            {
                return NotFound(); // Returns 404 if the skill doesn't exist
            }

            return Ok(skill); // Returns 200 OK with the skill data
        }

        // POST: api/Skills
        // Creates a new skill.
        [HttpPost]
        public async Task<ActionResult<Skill>> PostSkill(SkillDto skillDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Important: Check if the associated employee exists before adding a skill.
            if (!await _context.Employees.AnyAsync(e => e.EmployeeID == skillDto.EmployeeID))
            {
                return NotFound($"Employee with ID {skillDto.EmployeeID} not found.");
            }

            var skill = new Skill
            {
                EmployeeID = skillDto.EmployeeID,
                SkillName = skillDto.SkillName,
                Description = skillDto.Description,
                Level = skillDto.Level
            };

            _context.Skills.Add(skill);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSkill), new { id = skill.SkillID }, skill);
        }

        // PUT: api/Skills/5
        // Updates an existing skill.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSkill(int id, SkillDto skillDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var skillToUpdate = await _context.Skills.FindAsync(id);

            if (skillToUpdate == null)
            {
                return NotFound();
            }

            // Update properties from the DTO
            skillToUpdate.SkillName = skillDto.SkillName;
            skillToUpdate.Description = skillDto.Description;
            skillToUpdate.Level = skillDto.Level;

            _context.Entry(skillToUpdate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Skills.Any(e => e.SkillID == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Skills/5
        // Deletes a skill.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(int id)
        {
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound();
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}