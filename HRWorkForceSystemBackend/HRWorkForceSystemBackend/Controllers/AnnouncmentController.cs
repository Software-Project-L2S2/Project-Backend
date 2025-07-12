// File: Controllers/AnnouncementsController.cs

using Microsoft.AspNetCore.Mvc;
using HRWorkForceSystemBackend.Services; // Import the service
using HRWorkforceSystemBackend.DTOs;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class AnnouncementsController : ControllerBase
{
    private readonly AnnouncementService _announcementService;

    // Inject the service instead of the DbContext
    public AnnouncementsController(AnnouncementService announcementService)
    {
        _announcementService = announcementService;
    }

    [HttpGet("for-user/{role}")]
    public async Task<IActionResult> GetAnnouncementsForUser(string role)
    {
        var announcements = await _announcementService.GetAnnouncementsForUserAsync(role);
        return Ok(announcements);
    }

    [HttpPost]
    public async Task<IActionResult> PostAnnouncement([FromBody] AnnouncementCreateDto dto)
    {
        try
        {
            var createdAnnouncement = await _announcementService.CreateAnnouncementAsync(dto);
            return CreatedAtAction(nameof(GetAnnouncementsForUser), new { role = "All" }, createdAnnouncement);
        }
        catch (System.Exception ex)
        {
            // Catch potential exceptions from the service (e.g., sender not found)
            return BadRequest(ex.Message);
        }
    }
}