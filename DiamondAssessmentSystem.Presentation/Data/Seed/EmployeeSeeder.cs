using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class EmployeeSeeder
    {
        public static List<Employee> Employees = new();

        private static readonly string[] EmployeeRoles = new[] { "Admin", "Manager", "Consultant", "Assessor" };

        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.Employees.Any()) return;

            var users = UserSeeder.Users.Where(u => EmployeeRoles.Contains(u.UserType)).ToList();

            for (int i = 0; i < users.Count; i++)
            {
                Employees.Add(new Employee
                {
                    UserId = users[i].Id,
                    Salary = 1000 + i * 100
                });
            }

            context.Employees.AddRange(Employees);
            context.SaveChanges();
        }
    }
}
