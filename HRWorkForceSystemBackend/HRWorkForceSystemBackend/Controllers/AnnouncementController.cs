using Microsoft.AspNetCore.Mvc;
using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models;
using HRWorkForceSystemBackend.Services;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
#pragma warning disable CA1050 // Declare types in namespaces
public class AnnouncementsController : ControllerBase
#pragma warning restore CA1050 // Declare types in namespaces
{
    private readonly AppDbContext _context;
    private readonly EmailService _emailService;
    
    public AnnouncementsController(AppDbContext context, EmailService emailService)
    {
        _context = context;
        _emailService = emailService;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Announcement>>> GetAnnouncements()
    {
        return await _context.Announcements.ToListAsync();
    }
    
    [HttpGet("roles")]
    public async Task<ActionResult<IEnumerable<string>>> GetAvailableRoles()
    {
        var roles = new List<string> { "All", "HRUsers", "Admins", "WorkforceUsers" };
        return Ok(roles);
    }
    
    [HttpPost]
    public async Task<ActionResult<Announcement>> AddAnnouncement(Announcement announcement)
    {
        try
        {
            // Save announcement to database
            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();
            
            // Send emails if communication type is email
            if (announcement.CommunicationType?.ToLower() == "email")
            {
                await SendAnnouncementEmails(announcement);
            }
            
            return CreatedAtAction(nameof(GetAnnouncements), new { id = announcement.Id }, announcement);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error creating announcement: {ex.Message}");
        }
    }
    
    private async Task SendAnnouncementEmails(Announcement announcement)
    {
        List<string> emailAddresses = new List<string>();
        
        try
        {
            if (announcement.Audience == "All")
            {
                // Get all employees from all tables
                var adminEmails = await _context.Admins.Select(a => a.Email).ToListAsync();
                var hrEmails = await _context.HRUsers.Select(h => h.Email).ToListAsync();
                var workforceEmails = await _context.WorkforceUsers.Select(w => w.Email).ToListAsync();
                
                emailAddresses.AddRange(adminEmails.Where(e => !string.IsNullOrEmpty(e)));
                emailAddresses.AddRange(hrEmails.Where(e => !string.IsNullOrEmpty(e)));
                emailAddresses.AddRange(workforceEmails.Where(e => !string.IsNullOrEmpty(e)));
            }
            else
            {
                // Get emails based on specific role
                switch (announcement.Audience)
                {
                    case "Admins":
                        emailAddresses = await _context.Admins
                            .Where(a => !string.IsNullOrEmpty(a.Email))
                            .Select(a => a.Email)
                            .ToListAsync();
                        break;
                        
                    case "HRUsers":
                        emailAddresses = await _context.HRUsers
                            .Where(h => !string.IsNullOrEmpty(h.Email))
                            .Select(h => h.Email)
                            .ToListAsync();
                        break;
                        
                    case "WorkforceUsers":
                        emailAddresses = await _context.WorkforceUsers
                            .Where(w => !string.IsNullOrEmpty(w.Email))
                            .Select(w => w.Email)
                            .ToListAsync();
                        break;
                }
            }
            
            // Remove duplicates
            emailAddresses = emailAddresses.Distinct().ToList();
            
            // Convert to Sri Lanka time zone (UTC+5:30)
            var sriLankaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");
            var sriLankaTime = TimeZoneInfo.ConvertTimeFromUtc(announcement.CreatedAt.ToUniversalTime(), sriLankaTimeZone);
            
            // Send emails
            string subject = $"Announcement: {announcement.Title}";
            string body = $@"
                <html>
                <body>
                    <h2>{announcement.Title}</h2>
                    <p><strong>Target Audience:</strong> {announcement.Audience}</p>
                    {(!string.IsNullOrEmpty(announcement.TagRole) ? $"<p><strong>Role Tag:</strong> {announcement.TagRole}</p>" : "")}
                    <div>
                        <p>{announcement.Note}</p>
                    </div>
                    <hr>
                    <p><em>Sent on: {sriLankaTime:yyyy-MM-dd}</em></p>
                </body>
                </html>";
            
            foreach (var email in emailAddresses)
            {
                try
                {
                    await _emailService.SendEmailAsync(email, subject, body);
                }
                catch (Exception ex)
                {
                    // Log individual email failures but continue with others
                    Console.WriteLine($"Failed to send email to {email}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending announcement emails: {ex.Message}");
            throw;
        }
    }
}