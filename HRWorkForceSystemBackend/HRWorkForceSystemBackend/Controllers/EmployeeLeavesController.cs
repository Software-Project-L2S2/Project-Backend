using Microsoft.AspNetCore.Mvc;
using HRWorkForceSystemBackend.Models.LeaveModels;
using HRWorkForceSystemBackend.Data;
using Microsoft.EntityFrameworkCore;

namespace HRWorkForceSystemBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeLeavesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeLeavesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("{employeeId}")]
        public async Task<ActionResult<EmployeeLeave>> GetEmployeeLeave(string employeeId)
        {
            var employeeLeave = await _context.EmployeeLeaves
                .FirstOrDefaultAsync(el => el.EmployeeId == employeeId);

            return employeeLeave ?? new EmployeeLeave { EmployeeId = employeeId };
        }
    }
}