using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class UserRoleSeeder
    {
        public static async Task SeedUserRolesAsync(
            UserManager<User> userManager,
            ILogger logger = null)
        {
            foreach (var user in UserSeeder.Users)
            {
                var roles = await userManager.GetRolesAsync(user);
                if (!roles.Contains(user.UserType))
                {
                    var result = await userManager.AddToRoleAsync(user, user.UserType);
                    if (result.Succeeded)
                    {
                        logger?.LogInformation($"Assigned user {user.UserName} to role {user.UserType}.");
                    }
                    else
                    {
                        logger?.LogError($"Failed to assign role {user.UserType} to {user.UserName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }
    }
}
