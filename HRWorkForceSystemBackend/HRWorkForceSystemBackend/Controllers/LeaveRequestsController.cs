using Microsoft.AspNetCore.Mvc;
using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models.LeaveModels;
using Microsoft.EntityFrameworkCore;

namespace HRWorkForceSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LeaveRequestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetLeaveRequests([FromQuery] string type = "all")
        {
            var query = _context.LeaveRequests
                .Join(_context.EmployeeLeaves,
                    lr => lr.EmployeeId,
                    el => el.EmployeeId,
                    (lr, el) => new {
                        lr.Id,
                        lr.EmployeeId,
                        lr.EmployeeName,
                        lr.StartDate,
                        lr.EndDate,
                        lr.LeaveType,
                        lr.Status,
                        el.AvailableLeaves
                    });

            var typeMapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["annual"] = "Annual Leave",
                ["sick"] = "Sick Leave",
                ["personal"] = "Personal Leave"
            };

            if (!string.IsNullOrEmpty(type) && type.ToLower() != "all")
            {
                if (typeMapping.TryGetValue(type.ToLower(), out var mappedType))
                {
                    query = query.Where(r => r.LeaveType == mappedType);
                }
            }

            return await query.ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<LeaveRequest>> PostLeaveRequest(LeaveRequest request)
        {
            var employeeLeave = await _context.EmployeeLeaves
                .FirstOrDefaultAsync(el => el.EmployeeId == request.EmployeeId);

            if (employeeLeave == null)
            {
                _context.EmployeeLeaves.Add(new EmployeeLeave 
                { 
                    EmployeeId = request.EmployeeId 
                });
            }

            request.Status = "Pending";
            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeaveRequests", new { id = request.Id }, request);
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> ApproveLeaveRequest(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null) return NotFound();

            var employeeLeave = await _context.EmployeeLeaves
                .FirstOrDefaultAsync(el => el.EmployeeId == request.EmployeeId);

            if (employeeLeave == null) return BadRequest("Employee record not found");
            if (employeeLeave.AvailableLeaves < request.LeaveDays)
                return BadRequest("Insufficient available leaves");

            employeeLeave.LeavesTaken += request.LeaveDays;
            request.Status = "Approved";
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectLeaveRequest(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "Rejected";
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaveRequestExists(int id)
        {
            return _context.LeaveRequests.Any(e => e.Id == id);
        }
    }
}