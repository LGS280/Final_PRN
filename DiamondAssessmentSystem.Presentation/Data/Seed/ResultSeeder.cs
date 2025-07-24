using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class ResultSeeder
    {
        public static List<Result> Results = new();

        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.Results.Any()) return;

            var completedOrders = OrderSeeder.Orders
                .Where(o => o.Status == "Completed")
                .ToList();

            var completedRequestIds = completedOrders
                .Select(o => context.Requests.FirstOrDefault(r => r.CustomerId == o.CustomerId && r.ServiceId == o.ServiceId)?.RequestId)
                .Where(id => id != null)
                .Distinct()
                .ToList();

            var random = new Random();
            int index = 0;

            foreach (var requestId in completedRequestIds)
            {
                if (requestId == null) continue;

                var status = (index % 2 == 0) ? "Completed" : "InProgress";

                Results.Add(new Result
                {
                    RequestId = requestId.Value,
                    DiamondId = 1000 + index,
                    DiamondOrigin = "South Africa",
                    Shape = (index % 3 == 0) ? "Round" : (index % 3 == 1) ? "Princess" : "Oval",
                    Measurements = "5.10 x 5.12 x 3.15 mm",
                    CaratWeight = 0.8m + (index % 5) * 0.15m,
                    Color = "F",
                    Clarity = "VS2",
                    Cut = "Excellent",
                    Proportions = "60%",
                    Polish = "Excellent",
                    Symmetry = "Very Good",
                    Fluorescence = "None",
                    ModifiedDate = DateTime.Now.AddDays(-index),
                    Status = status
                });

                index++;
            }

            context.Results.AddRange(Results);
            context.SaveChanges();
        }
    }
}
