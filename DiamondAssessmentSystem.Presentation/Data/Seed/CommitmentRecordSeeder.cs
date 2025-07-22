using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class CommitmentRecordSeeder
    {
        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.CommitmentRecords.Any()) return;

            var requests = RequestSeeder.Requests
                .Where(r => r.Status == "Pending")
                .Take(15)
                .ToList();

            var assessorIds = context.Users
                .Where(u => u.UserType == "Assessor")
                .Join(context.Employees, u => u.Id, e => e.UserId, (u, e) => e.EmployeeId)
                .ToList();

            if (!assessorIds.Any())
                throw new Exception("No Assessor found to approve commitment records.");

            var records = new List<CommitmentRecord>();
            var rand = new Random();

            for (int i = 0; i < requests.Count; i++)
            {
                records.Add(new CommitmentRecord
                {
                    RequestId = requests[i].RequestId,
                    CommitDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-i)),
                    CommitmentReason = $"Commitment reason #{i + 1}",
                    ApprovedBy = assessorIds[i % assessorIds.Count],
                    ApprovedDate = DateTime.Now.AddDays(-i),
                    Status = "Approved"
                });
            }

            context.CommitmentRecords.AddRange(records);
            context.SaveChanges();
        }
    }
}
