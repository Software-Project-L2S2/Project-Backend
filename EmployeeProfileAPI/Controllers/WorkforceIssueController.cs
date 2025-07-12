using Microsoft.AspNetCore.Mvc;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class WorkforceIssuesController : ControllerBase
{
    private readonly AppDbContext _context;

    public WorkforceIssuesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkforceIssue>>> GetWorkforceIssues()
    {
        return await _context.WorkforceIssues.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<WorkforceIssue>> AddWorkforceIssue(WorkforceIssue issue)
    {
        _context.WorkforceIssues.Add(issue);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetWorkforceIssues), new { id = issue.Id }, issue);
    }
}
