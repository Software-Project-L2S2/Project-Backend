// File: Data/DbInitializer.cs
// This is your file, edited to match the new Announcement model

using HRWorkForceSystemBackend.Data;
using HRWorkForceSystemBackend.Models;
using HRWorkForceSystemBackend.Models.AuthModels; // Required for Admin, HRUser, etc.
using System.Linq;

namespace HRWorkforceSystemBackend.Data // Your original namespace
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            context.Database.EnsureCreated();

            // Seed users only if all user tables are empty to avoid partial seeding
            if (context.Admins.Any() || context.HRUsers.Any() || context.WorkforceUsers.Any())
            {
                // return; // Or just skip user seeding
            }
            else
            {
                // Using your user models
                var admin = new Admin { FirstName = "Admin", LastName = "User" /*, other required fields */ };
                context.Admins.Add(admin);

                var hrUser = new HRUser { FirstName = "HR", LastName = "Manager" /*, etc. */ };
                context.HRUsers.Add(hrUser);

                context.WorkforceUsers.Add(new WorkforceUser { FirstName = "John", LastName = "Doe" /*, etc. */ });
                context.WorkforceUsers.Add(new WorkforceUser { FirstName = "Jane", LastName = "Smith" /*, etc. */ });

                context.SaveChanges();
            }


            if (context.Announcements.Any())
            {
                return;   // DB has been seeded with announcements
            }

            // Seed announcements with corrected foreign keys
            var announcements = new Announcement[]
            {
                new Announcement
                {
                    Title = "Welcome!",
                    Content = "Welcome to the new announcement system.",
                    TargetRole = "All",
                    AdminSenderId = 1 // Sent by Admin with ID 1
                },
                new Announcement
                {
                    Title = "Q3 Performance Reviews",
                    Content = "Please complete your self-assessment for Q3 reviews by the end of the month.",
                    TargetRole = "Workforce",
                    HRUserSenderId = 1 // Sent by HRUser with ID 1
                },
            };

            foreach (Announcement a in announcements)
            {
                context.Announcements.Add(a);
            }
            context.SaveChanges();
        }
    }
}