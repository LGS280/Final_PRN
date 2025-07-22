using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class UserSeeder
    {
        public static List<User> Users = new();

        private static readonly List<string> Roles = new()
        {
            "Customer", "Manager", "Consultant", "Assessor"
        };

        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.Users.Any()) return;

            int globalIndex = 1;

            foreach (var role in Roles)
            {
                for (int i = 1; i <= 10; i++)
                {
                    var id = $"user{globalIndex}";
                    Users.Add(new User
                    {
                        Id = id,
                        UserName = $"{role.ToLower()}{i}@gmail.com",
                        Email = $"{role.ToLower()}{i}@gmail.com",
                        UserType = role,
                        FirstName = $"{role}{i}",
                        LastName = "User",
                        Gender = (i % 2 == 0) ? "Male" : "Female",
                        Point = role == "Customer" ? i * 10 : null,
                        DateCreated = DateTime.Now.AddDays(-globalIndex),
                        Status = "active"
                    });

                    globalIndex++;
                }
            }

            context.Users.AddRange(Users);
            context.SaveChanges();
        }
    }
}
