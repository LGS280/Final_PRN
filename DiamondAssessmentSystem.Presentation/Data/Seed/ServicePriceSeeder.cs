using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class ServicePriceSeeder
    {
        public static List<ServicePrice> Services = new();

        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.ServicePrices.Any()) return;

            var employees = EmployeeSeeder.Employees;

            var serviceTypes = new[] { "Basic Appraisal", "Advanced Appraisal", "Certification", "Cleaning", "Engraving" };

            for (int i = 0; i < 10; i++)
            {
                var employee = employees[i % employees.Count];

                Services.Add(new ServicePrice
                {
                    ServiceType = serviceTypes[i % serviceTypes.Length],
                    Description = $"Detailed description for {serviceTypes[i % serviceTypes.Length]}",
                    Price = 100 + i * 50,
                    Duration = 1 + i % 5,
                    DateCreated = DateTime.Now.AddDays(-i),
                    EmployeeId = employee.EmployeeId,
                    Status = "active"
                });
            }

            context.ServicePrices.AddRange(Services);
            context.SaveChanges();
        }
    }
}
