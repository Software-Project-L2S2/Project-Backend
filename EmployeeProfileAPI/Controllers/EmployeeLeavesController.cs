using Microsoft.AspNetCore.Mvc;
using EmployeeProfileAPI.Models;
using EmployeeProfileAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeProfileAPI.Controllers
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