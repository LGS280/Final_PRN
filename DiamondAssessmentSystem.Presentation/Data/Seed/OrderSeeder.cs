using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class OrderSeeder
    {
        public static List<Order> Orders = new();

        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.Orders.Any()) return;

            var requests = RequestSeeder.Requests
                .Where(r => r.Status == "Pending")
                .ToList();

            var services = ServicePriceSeeder.Services;
            var customers = CustomerSeeder.Customers;

            var random = new Random();
            var paymentTypes = new[] { "Online", "Offline" };

            int orderIndex = 0;

            foreach (var request in requests.Take(20)) 
            {
                var service = services.FirstOrDefault(s => s.ServiceId == request.ServiceId);
                var customer = customers.FirstOrDefault(c => c.CustomerId == request.CustomerId);

                var paymentMethod = paymentTypes[orderIndex % 2]; 

                var order = new Order
                {
                    CustomerId = customer.CustomerId,
                    ServiceId = service.ServiceId,
                    OrderDate = DateTime.Now.AddDays(-orderIndex),
                    TotalPrice = service.Price,
                    Status = paymentMethod == "Online" ? "Completed" : "Pending"
                };

                Orders.Add(order);
                orderIndex++;
            }

            context.Orders.AddRange(Orders);
            context.SaveChanges();
        }
    }
}
