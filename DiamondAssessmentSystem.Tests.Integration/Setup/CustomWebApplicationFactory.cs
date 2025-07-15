using DiamondAssessmentSystem;
using DiamondAssessmentSystem.Infrastructure;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.EntityFrameworkCore.InMemory;

namespace DiamondAssessmentSystem.Tests.Integration.Setup
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Xóa cấu hình DbContext thật
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<DiamondAssessmentDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Thêm DbContext InMemory cho test
                services.AddDbContext<DiamondAssessmentDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                // Build service provider
                var sp = services.BuildServiceProvider();

                // Seed data test
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<DiamondAssessmentDbContext>();

                    db.Database.EnsureCreated();

                    try
                    {
                        DbSeeder.Seed(db);
                    }
                    catch (Exception ex)
                    {
                        // Logging
                        Console.WriteLine($"Error seeding test database: {ex.Message}");
                    }
                }
            });
        }
    }
}
