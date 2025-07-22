using DiamondAssessmentSystem.Infrastructure.Models;
using DiamondAssessmentSystem.Presentation.Data.Seed;

namespace DiamondAssessmentSystem.Presentation.Data
{
    public static class DbInitializer
    {
        public static void Seed(DiamondAssessmentDbContext context)
        {
            UserSeeder.Seed(context);
            EmployeeSeeder.Seed(context);
            CustomerSeeder.Seed(context);
            ServicePriceSeeder.Seed(context);

            RequestSeeder.Seed(context);
            OrderSeeder.Seed(context);
            PaymentSeeder.Seed(context);

            ResultSeeder.Seed(context);
            CertificateSeeder.Seed(context);
            SealingRecordSeeder.Seed(context);
            CommitmentRecordSeeder.Seed(context);
            ServicePriceAuditSeeder.Seed(context);
        }
    }
}
