using Microsoft.AspNetCore.Identity;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class RoleSeeder
    {
        private static readonly List<string> DefaultRoles = new()
        {
            "Admin",
            "Customer",
            "Manager",
            "Consultant",
            "Assessor"
        };

        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager, ILogger logger = null)
        {
            foreach (var role in DefaultRoles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));

                    if (result.Succeeded)
                    {
                        logger?.LogInformation($"Created role: {role}");
                    }
                    else
                    {
                        logger?.LogError($"Failed to create role: {role}. Errors: {string.Join(", ", result.Errors)}");
                    }
                }
                else
                {
                    logger?.LogInformation($"Role already exists: {role}");
                }
            }
        }
    }
}
