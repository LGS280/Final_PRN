using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class RequestSeeder
    {
        public static List<Request> Requests = new();

        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.Requests.Any()) return;

            var customers = CustomerSeeder.Customers;
            var services = ServicePriceSeeder.Services;

            var consultantEmployeeIds = context.Users
                .Where(u => u.UserType == "Consultant")
                .Join(context.Employees, u => u.Id, e => e.UserId, (u, e) => e.EmployeeId)
                .ToList();

            if (consultantEmployeeIds.Count == 0)
                throw new Exception("No Consultant employee found for assigning to requests.");

            var random = new Random();

            for (int i = 0; i < 30; i++)
            {
                var customer = customers[i % customers.Count];
                var service = services[i % services.Count];
                var consultantId = consultantEmployeeIds[i % consultantEmployeeIds.Count];

                var status = (i % 2 == 0) ? "Draft" : "Pending";

                Requests.Add(new Request
                {
                    CustomerId = customer.CustomerId,
                    ServiceId = service.ServiceId,
                    EmployeeId = consultantId,
                    RequestDate = DateTime.Now.AddDays(-i),
                    RequestType = (random.Next(2) == 0) ? "Standard" : "Urgent",
                    Status = status
                });
            }

            context.Requests.AddRange(Requests);
            context.SaveChanges();
        }
    }
}
