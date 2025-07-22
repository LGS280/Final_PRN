using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class CertificateSeeder
    {
        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.Certificates.Any()) return;

            var results = ResultSeeder.Results
                .Where(r => r.Status == "Completed")
                .ToList();

            var employees = EmployeeSeeder.Employees;
            var certificates = new List<Certificate>();

            for (int i = 0; i < results.Count; i++)
            {
                certificates.Add(new Certificate
                {
                    ResultId = results[i].ResultId,
                    CertificateNumber = $"CERT-{1000 + i}",
                    IssueDate = DateTime.Now.AddDays(-i),
                    ApprovedBy = employees[i % employees.Count].EmployeeId,
                    ApprovedDate = DateTime.Now.AddDays(-i),
                    Status = "Issued"
                });
            }

            context.Certificates.AddRange(certificates);
            context.SaveChanges();
        }
    }
}
