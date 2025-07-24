using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class PaymentSeeder
    {
        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.Payments.Any()) return;

            var orders = OrderSeeder.Orders;

            var methods = new[] { "VNPay", "BankTransfer", "Cash" };
            var payments = new List<Payment>();

            foreach (var order in orders)
            {
                if (order.Status != "Completed") continue;

                payments.Add(new Payment
                {
                    OrderId = order.OrderId,
                    PaymentDate = order.OrderDate.AddDays(1),
                    Amount = order.TotalPrice,
                    Method = methods[order.OrderId % methods.Length],
                    Status = "successful"
                });
            }

            context.Payments.AddRange(payments);
            context.SaveChanges();
        }
    }
}
