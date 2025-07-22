using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class AdminSeeder
    {
        public static async Task SeedAdminAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration,
            ILogger logger = null)
        {
            var adminConfig = configuration.GetSection("DefaultAdmin");
            var username = adminConfig["Username"];
            var email = adminConfig["Email"];
            var password = adminConfig["Password"];
            var role = adminConfig["Role"];
            var firstName = adminConfig["FirstName"];
            var lastName = adminConfig["LastName"];
            var phone = adminConfig["PhoneNumber"];

            // Kiểm tra nếu đã tồn tại User có role Admin
            var existingUser = await userManager.FindByNameAsync(username);
            if (existingUser != null)
            {
                logger?.LogInformation("Admin already exists.");
                return;
            }

            var admin = new User
            {
                UserName = username,
                Email = email,
                EmailConfirmed = true,
                PhoneNumber = phone,
                UserType = role,
                FirstName = firstName,
                LastName = lastName,
                Status = "active",
                DateCreated = DateTime.Now
            };

            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
            {
                logger?.LogError($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return;
            }

            await userManager.AddToRoleAsync(admin, role);
            logger?.LogInformation("Admin user created and assigned to role.");

            // Tạo Employee mapping tương ứng
            using var scope = Program.ServiceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DiamondAssessmentDbContext>();

            if (!context.Employees.Any(e => e.UserId == admin.Id))
            {
                context.Employees.Add(new Employee
                {
                    UserId = admin.Id,
                    Salary = 1000
                });
                await context.SaveChangesAsync();
                logger?.LogInformation("Mapped Admin user to Employee.");
            }
        }
    }
}
