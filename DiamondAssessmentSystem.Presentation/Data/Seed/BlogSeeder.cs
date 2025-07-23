using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.Extensions.Configuration;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class BlogSeeder
    {
        public static void Seed(DiamondAssessmentDbContext context, IConfiguration configuration)
        {
            if (context.Blogs.Any()) return;

            var adminUsername = configuration["DefaultAdmin:Username"];

            var adminUser = context.Users.FirstOrDefault(u => u.UserName == adminUsername);
            if (adminUser == null)
                throw new Exception("Admin user not found. Make sure AdminSeeder was run before BlogSeeder.");

            var adminEmployee = context.Employees.FirstOrDefault(e => e.UserId == adminUser.Id);
            if (adminEmployee == null)
                throw new Exception("Admin user is not mapped to an Employee. Cannot assign Blog author.");

            var blogTypes = new[] { "News", "Guide", "Update", "Tips", "Tutorial" };
            var blogs = new List<Blog>();

            for (int i = 1; i <= 50; i++)
            {
                blogs.Add(new Blog
                {
                    Title = $"Diamond Insight #{i}",
                    Content = $"<p>This is auto-generated blog #{i} providing valuable insights about diamond certification, appraisal, and care.</p>",
                    ImageUrl = $"https://images.pexels.com/photos/839443/pexels-photo-839443.jpeg",
                    BlogType = blogTypes[i % blogTypes.Length],
                    Status = "Published",
                    CreatedDate = DateTime.Now.AddDays(-i),
                    UpdatedDate = null,
                    EmployeeId = adminEmployee.EmployeeId
                });
            }

            context.Blogs.AddRange(blogs);
            context.SaveChanges();
        }
    }
}
