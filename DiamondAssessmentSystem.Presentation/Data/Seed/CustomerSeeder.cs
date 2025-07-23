using DiamondAssessmentSystem.Infrastructure.Models;

namespace DiamondAssessmentSystem.Presentation.Data.Seed
{
    public static class CustomerSeeder
    {
        public static List<Customer> Customers = new();

        public static void Seed(DiamondAssessmentDbContext context)
        {
            if (context.Customers.Any()) return;

            var users = UserSeeder.Users.Where(u => u.UserType == "Customer").ToList();

            for (int i = 0; i < users.Count; i++)
            {
                Customers.Add(new Customer
                {
                    UserId = users[i].Id,
                    Idcard = 1000000000 + i,
                    Address = $"123 Street {i}",
                    UnitName = $"Unit {i}",
                    TaxCode = $"TX{i:D5}"
                });
            }

            context.Customers.AddRange(Customers);
            context.SaveChanges();
        }
    }
}
