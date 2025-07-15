using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.SeedData
{
    public static class DbInitializer
    {
        public static async Task SeedDefaultAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            var adminConfig = configuration.GetSection("DefaultAdmin");
            var username = adminConfig["Username"];
            var email = adminConfig["Email"];
            var password = adminConfig["Password"];
            var firstName = adminConfig["FirstName"];
            var lastName = adminConfig["LastName"];
            var phoneNumber = adminConfig["PhoneNumber"];
            var role = adminConfig["Role"];

            // Check nếu đã tồn tại user
            var existingUser = await userManager.FindByNameAsync(username);
            if (existingUser != null) return;

            // Tạo role nếu chưa có
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }

            // Tạo user mới
            var user = new User
            {
                UserName = username,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                UserType = "Admin",
                Status = "Active",
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                throw new Exception("Seed admin failed: " + string.Join("; ", createResult.Errors));

            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                throw new Exception("Assign admin role failed: " + string.Join("; ", roleResult.Errors));
        }
    }
}
