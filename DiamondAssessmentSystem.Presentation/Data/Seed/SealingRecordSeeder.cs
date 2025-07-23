using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class SealingRecordSeeder
    {
        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.SealingRecords.Any()) return;

            var completedResults = ResultSeeder.Results
                .Where(r => r.Status == "Completed")
                .Select(r => r.RequestId)
                .Distinct()
                .ToList();

            var requests = context.Requests
                .Where(r => completedResults.Contains(r.RequestId))
                .Take(10)
                .ToList();

            var managerIds = context.Users
                .Where(u => u.UserType == "Manager")
                .Join(context.Employees, u => u.Id, e => e.UserId, (u, e) => e.EmployeeId)
                .ToList();

            if (!managerIds.Any())
                throw new Exception("No Manager found to approve sealing records.");

            var records = new List<SealingRecord>();

            for (int i = 0; i < requests.Count; i++)
            {
                records.Add(new SealingRecord
                {
                    RequestId = requests[i].RequestId,
                    SealDate = DateTime.Now.AddDays(-i),
                    SealingReason = $"Sealing reason #{i + 1}",
                    ApprovedBy = managerIds[i % managerIds.Count],
                    ApprovedDate = DateTime.Now.AddDays(-i + 1),
                    Status = "Sealed"
                });
            }

            context.SealingRecords.AddRange(records);
            context.SaveChanges();
        }
    }
}
