using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using System.Threading.Tasks;

namespace EmployeeProfileAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileEducationController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileEducationController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/ProfileEducation
        [HttpPost]
        public async Task<ActionResult<ProfileEducation>> CreateProfileEducation(ProfileEducation profileEducation)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var employee = await _context.EmployeeProfiles.FindAsync(profileEducation.EmployeeID);
            if (employee == null)
            {
                return BadRequest(new { message = $"Employee with ID '{profileEducation.EmployeeID}' not found." });
            }

            _context.ProfileEducation.Add(profileEducation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProfileEducation", new { id = profileEducation.EducationID }, profileEducation);
        }

        // GET: api/ProfileEducation/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ProfileEducation>> GetProfileEducation(int id)
        {
            var profileEducation = await _context.ProfileEducation.FindAsync(id);

            if (profileEducation == null)
            {
                return NotFound();
            }

            return profileEducation;
        }

        // PUT: api/ProfileEducation/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProfileEducation(int id, ProfileEducation profileEducation)
        {
            if (id != profileEducation.EducationID)
            {
                return BadRequest();
            }

            _context.Entry(profileEducation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ProfileEducation.Any(e => e.EducationID == id))
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

        // DELETE: api/ProfileEducation/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProfileEducation(int id)
        {
            var profileEducation = await _context.ProfileEducation.FindAsync(id);
            if (profileEducation == null)
            {
                return NotFound();
            }

            _context.ProfileEducation.Remove(profileEducation);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}