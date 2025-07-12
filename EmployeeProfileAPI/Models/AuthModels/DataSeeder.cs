using Microsoft.EntityFrameworkCore;
using EmployeeProfileAPI.Data;

namespace EmployeeProfileAPI.Models.AuthModels
{
    public static class DataSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            if (await context.Admins.AnyAsync()) return;

            var email = "akmirrasmiya@gmail.com";
            var password = "Admin@123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var admin = new Admin
            {
                FirstName = "Rasmiya",
                LastName = "Akmirkan",
                Email = email,
                PasswordHash = hashedPassword,
                PhoneNumber = "0771263694"
            };

            await context.Admins.AddAsync(admin);
            await context.SaveChangesAsync();
        }

    }
}
