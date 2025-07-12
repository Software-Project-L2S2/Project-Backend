// File: Services/AnnouncementService.cs

using HRWorkForceSystemBackend.Data;          // Correct: For AppDbContext
using HRWorkForceSystemBackend.Models;        // Correct: For Announcement
using HRWorkForceSystemBackend.Models.AuthModels; // Correct: For Admin and HRUser
using HRWorkforceSystemBackend.DTOs;          // Correct: For our DTOs
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HRWorkForceSystemBackend.Services
{
    public class AnnouncementService
    {
        // Corrected to use AppDbContext
        private readonly AppDbContext _context;

        // The constructor now correctly injects AppDbContext
        public AnnouncementService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Creates a new announcement and saves it to the database.
        /// </summary>
        /// <param name="dto">The data transfer object containing announcement details.</param>
        /// <returns>The newly created announcement as a view DTO.</returns>
        public async Task<AnnouncementViewDto> CreateAnnouncementAsync(AnnouncementCreateDto dto)
        {
            var announcement = new Announcement
            {
                Title = dto.Title,
                Content = dto.Content,
                TargetRole = dto.TargetRole
            };

            // This logic correctly sets one of the two foreign keys, not both.
            if (dto.SenderRole == "Admin")
            {
                var sender = await _context.Admins.FindAsync(dto.SenderId);
                if (sender == null) throw new KeyNotFoundException("Admin sender with the specified ID was not found.");

                announcement.AdminSenderId = dto.SenderId;
                announcement.HRUserSenderId = null; // Ensure the other FK is null
            }
            else if (dto.SenderRole == "HR")
            {
                var sender = await _context.HRUsers.FindAsync(dto.SenderId);
                if (sender == null) throw new KeyNotFoundException("HR sender with the specified ID was not found.");

                announcement.HRUserSenderId = dto.SenderId;
                announcement.AdminSenderId = null; // Ensure the other FK is null
            }
            else
            {
                throw new ArgumentException("Invalid sender role. Must be 'Admin' or 'HR'.");
            }

            _context.Announcements.Add(announcement);
            await _context.SaveChangesAsync();

            // After saving, we can construct the View DTO to return to the user.
            // We need to reload the entry to get the Sender's details if we want them.
            await _context.Entry(announcement).Reference(a => a.AdminSender).LoadAsync();
            await _context.Entry(announcement).Reference(a => a.HRUserSender).LoadAsync();

            return new AnnouncementViewDto
            {
                Id = announcement.Id,
                Title = announcement.Title,
                Content = announcement.Content,
                CreatedAt = announcement.CreatedAt,
                TargetRole = announcement.TargetRole,
                Sender = announcement.AdminSender != null
                    ? new SenderDto { FirstName = announcement.AdminSender.FirstName, Role = "Admin" }
                    : new SenderDto { FirstName = announcement.HRUserSender.FirstName, Role = "HR" }
            };
        }

        /// <summary>
        /// Gets all announcements visible to a user with a specific role.
        /// </summary>
        /// <param name="userRole">The role of the user (e.g., "Workforce", "HR", "Admin").</param>
        /// <returns>A list of announcements formatted for viewing.</returns>
        public async Task<List<AnnouncementViewDto>> GetAnnouncementsForUserAsync(string userRole)
        {
            if (string.IsNullOrEmpty(userRole))
            {
                throw new ArgumentException("User role cannot be null or empty.");
            }

            var announcements = await _context.Announcements
                .Include(a => a.AdminSender)  // Eager load Admin sender details
                .Include(a => a.HRUserSender)   // Eager load HR sender details
                                                // Filter where the target is "All" OR matches the user's specific role
                .Where(a => a.TargetRole == "All" || a.TargetRole == userRole)
                .OrderByDescending(a => a.CreatedAt)
                // Project the database model to the View DTO
                .Select(a => new AnnouncementViewDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    CreatedAt = a.CreatedAt,
                    TargetRole = a.TargetRole,
                    // Use the sender's details from whichever navigation property is not null
                    Sender = a.AdminSender != null
                        ? new SenderDto { FirstName = a.AdminSender.FirstName, Role = "Admin" }
                        : a.HRUserSender != null
                            ? new SenderDto { FirstName = a.HRUserSender.FirstName, Role = "HR" }
                            : new SenderDto { FirstName = "System", Role = "System" } // Fallback
                })
                .ToListAsync();

            return announcements;
        }
    }
}