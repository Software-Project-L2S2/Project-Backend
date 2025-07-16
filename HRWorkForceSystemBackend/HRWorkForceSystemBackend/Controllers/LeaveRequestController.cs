using Microsoft.AspNetCore.Mvc;
using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models.LeaveModels;
using HRWorkForceSystemBackend.DTOs.LeaveRequestDTOs;

namespace HRWorkForceSystemBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public LeaveRequestController(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> SubmitLeave([FromForm] CreateLeaveRequestDto dto)
        {
            string? filePath = null;
            if (dto.Document != null && dto.Document.Length > 0)
{
    // ✅ Sanitize file name to prevent path traversal attacks or invalid chars
    var originalFileName = Path.GetFileName(dto.Document.FileName); 
    var uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";

    // ✅ Create path
    var path = Path.Combine(_env.WebRootPath, "documents");
    Directory.CreateDirectory(path); // ensures the folder exists

    // ✅ Combine final full path for saving
    var fullPath = Path.Combine(path, uniqueFileName);

    // ✅ Save file
    using var stream = new FileStream(fullPath, FileMode.Create);
    await dto.Document.CopyToAsync(stream);

    // ✅ Store relative path to return/store in DB
    filePath = Path.Combine("documents", uniqueFileName);
}


            var request = new LeaveRequest
            {
                LeaveType = dto.LeaveType,
                Reason = dto.Reason,
                EmployeeName = dto.EmployeeName,
                EmployeeId = dto.EmployeeId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                DocumentPath = filePath
            };

            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync();
            return Ok(request);
        }

        [HttpGet]
        public IActionResult GetAllRequests() =>
            Ok(_context.LeaveRequests.OrderByDescending(r => r.CreatedAt).ToList());

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = "Approved";
            await _context.SaveChangesAsync();
            return Ok(request);
        }

        [HttpPut("{id}/decline")]
        public async Task<IActionResult> Decline(int id, [FromQuery] string reason)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null) return NotFound();

            request.Status = $"Declined: {reason}";
            await _context.SaveChangesAsync();
            return Ok(request);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var request = await _context.LeaveRequests.FindAsync(id);
            if (request == null) return NotFound();

            _context.LeaveRequests.Remove(request);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
