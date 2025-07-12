using Microsoft.AspNetCore.Mvc;
using EmployeeProfileAPI.Data;
using EmployeeProfileAPI.Models;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class AnnouncementsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AnnouncementsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Announcement>>> GetAnnouncements()
    {
        return await _context.Announcements.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Announcement>> AddAnnouncement(Announcement announcement)
    {
        _context.Announcements.Add(announcement);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAnnouncements), new { id = announcement.Id }, announcement);
    }
}
