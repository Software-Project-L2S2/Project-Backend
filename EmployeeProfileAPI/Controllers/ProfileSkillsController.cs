using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using System.Threading.Tasks;

namespace EmployeeProfileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileSkillsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileSkillsController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/ProfileSkills
        [HttpPost]
        public async Task<ActionResult<ProfileSkill>> CreateProfileSkill(ProfileSkill profileSkill)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verify that the associated employee exists
            var employee = await _context.EmployeeProfiles.FindAsync(profileSkill.EmployeeID);
            if (employee == null)
            {
                // Return a specific error if the employee ID is not found
                return BadRequest(new { message = $"Employee with ID '{profileSkill.EmployeeID}' not found." });
            }

            _context.ProfileSkills.Add(profileSkill);
            await _context.SaveChangesAsync();

            // Return the created skill
            return CreatedAtAction("GetProfileSkill", new { id = profileSkill.SkillID }, profileSkill);
        }
        
        // GET: api/ProfileSkills/{id} - Helper endpoint, good to have
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileSkill>> GetProfileSkill(int id)
        {
            var profileSkill = await _context.ProfileSkills.FindAsync(id);

            if (profileSkill == null)
            {
                return NotFound();
            }

            return profileSkill;
        }


        // PUT: api/ProfileSkills/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfileSkill(int id, ProfileSkill profileSkill)
        {
            if (id != profileSkill.SkillID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(profileSkill).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ProfileSkills.Any(e => e.SkillID == id))
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

        // DELETE: api/ProfileSkills/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfileSkill(int id)
        {
            var profileSkill = await _context.ProfileSkills.FindAsync(id);
            if (profileSkill == null)
            {
                return NotFound();
            }

            _context.ProfileSkills.Remove(profileSkill);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}