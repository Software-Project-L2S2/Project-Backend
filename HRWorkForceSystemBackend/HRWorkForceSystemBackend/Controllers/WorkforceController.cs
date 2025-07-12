using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models.WorkforceModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRWorkForceSystemBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // Requires authentication for all endpoints
    public class WorkforceAnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkforceAnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/WorkforceAnalytics/movements
        [HttpGet("movements")]
        public async Task<ActionResult<IEnumerable<Movement>>> GetMovements()
        {
            return await _context.Movements.ToListAsync();
        }

        // GET: api/WorkforceAnalytics/attritions
        [HttpGet("attritions")]
        public async Task<ActionResult<IEnumerable<Attrition>>> GetAttritions()
        {
            return await _context.Attritions.ToListAsync();
        }

        // GET: api/WorkforceAnalytics/summary
        [HttpGet("summary")]
        public async Task<ActionResult<Summary>> GetSummary()
        {
            return new Summary
            {
                TotalPromotions = await _context.Movements.CountAsync(m => m.Status == "Promotion"),
                TotalExits = await _context.Movements.CountAsync(m => m.Status == "Exit"),
                TotalTransfers = await _context.Movements.CountAsync(m => m.Status == "Transfer"),
                TotalAttritions = await _context.Attritions.CountAsync()
            };
        }

        // POST: api/WorkforceAnalytics/movement
        [HttpPost("movement")]
        public async Task<ActionResult<Movement>> CreateMovement(Movement movement)
        {
            _context.Movements.Add(movement);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMovements), movement);
        }

        // POST: api/WorkforceAnalytics/attrition
        [HttpPost("attrition")]
        public async Task<ActionResult<Attrition>> CreateAttrition(Attrition attrition)
        {
            _context.Attritions.Add(attrition);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAttritions), attrition);
        }
    }
}