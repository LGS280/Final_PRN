using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class ServicePriceAuditSeeder
    {
        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.ServicePriceAudits.Any()) return;

            var services = ServicePriceSeeder.Services;
            var employees = EmployeeSeeder.Employees;

            var audits = new List<ServicePriceAudit>();

            for (int i = 0; i < services.Count; i++)
            {
                audits.Add(new ServicePriceAudit
                {
                    ServiceId = services[i].ServiceId,
                    ServiceType = services[i].ServiceType,
                    OldPrice = services[i].Price,
                    NewPrice = services[i].Price + 50,
                    OldDuration = services[i].Duration,
                    NewDuration = services[i].Duration + 1,
                    EmployeeId = employees[i % employees.Count].EmployeeId,
                    ChangeDate = DateTime.Now.AddDays(-i),
                    ActionType = "PriceUpdate",
                    Status = "approved"
                });
            }

            context.ServicePriceAudits.AddRange(audits);
            context.SaveChanges();
        }
    }
}
