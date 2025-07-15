using Bogus;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.SeedData
{
    public static partial class DataSeeder
    {
        private static readonly Faker faker = new Faker("en");

        public static async Task SeedSampleDataAsync(IServiceProvider serviceProvider, DiamondAssessmentDbContext dbContext)
        {
            using var scope = serviceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DataSeeder");

            if (dbContext.Users.Any(u => u.UserType != "Admin"))
            {
                logger.LogInformation("Non-admin users already exist. Skipping sample seeding.");
                return;
            }

            logger.LogInformation("No non-admin users. Seeding sample data...");

            // Seed Roles
            await RoleSeeder.SeedRolesAsync(roleManager, logger);

            // Seed Users
            var (customers, employees) = await SeedUsersAsync(userManager, roleManager, dbContext, logger);

            // Seed Service Prices
            var services = await SeedServicePricesAsync(dbContext, employees);

            // Seed Orders
            await SeedOrdersAsync(dbContext, customers, services);

            // Seed Requests
            var requests = await SeedRequestsAsync(dbContext, customers, employees, services);

            // Seed Results and Certificates
            await SeedResultsAndCertificatesAsync(dbContext, requests, employees);

            // Seed Conversations and Chats
            await SeedConversationsAndChatsAsync(dbContext, customers, employees);

            await dbContext.SaveChangesAsync();
        }

        private static async Task<(List<Customer>, List<Employee>)> SeedUsersAsync(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            DiamondAssessmentDbContext dbContext,
            ILogger logger
        )
        {
            var customers = new List<Customer>();
            var employees = new List<Employee>();

            // Seed Customers
            for (int i = 1; i <= 10; i++)
            {
                var username = $"customer{i}";
                if (await userManager.FindByNameAsync(username) != null) continue;

                var user = new User
                {
                    UserName = username,
                    Email = $"{username}@example.com",
                    FirstName = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    UserType = "Customer",
                    Status = "Active",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Customer@123");
                await userManager.AddToRoleAsync(user, "Customer");

                var customer = new Customer
                {
                    UserId = user.Id,
                    Idcard = faker.Random.Number(100000000, 999999999),
                    TaxCode = faker.Random.AlphaNumeric(10),
                    Address = faker.Address.FullAddress(),
                    UnitName = faker.Company.CompanyName()
                };

                dbContext.Customers.Add(customer);
                customers.Add(customer);

                logger?.LogInformation($"Seeded Customer: {username}");
            }

            // Seed Employees
            for (int i = 1; i <= 5; i++)
            {
                var username = $"employee{i}";
                if (await userManager.FindByNameAsync(username) != null) continue;

                var user = new User
                {
                    UserName = username,
                    Email = $"{username}@example.com",
                    FirstName = faker.Name.FirstName(),
                    LastName = faker.Name.LastName(),
                    UserType = "Employee",
                    Status = "Active",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user, "Employee@123");
                await userManager.AddToRoleAsync(user, "Assessor");

                var employee = new Employee
                {
                    UserId = user.Id,
                    Salary = faker.Random.Decimal(1000, 5000)
                };

                dbContext.Employees.Add(employee);
                employees.Add(employee);

                logger?.LogInformation($"Seeded Employee: {username}");
            }

            await dbContext.SaveChangesAsync();
            return (customers, employees);
        }

        private static async Task<List<ServicePrice>> SeedServicePricesAsync(DiamondAssessmentDbContext dbContext, List<Employee> employees)
        {
            var services = new List<ServicePrice>();
            var types = new[] { "Standard Report", "Premium Report", "Consulting", "Cleaning", "Laser Inscription" };

            foreach (var type in types)
            {
                var service = new ServicePrice
                {
                    ServiceType = type,
                    Price = faker.Random.Decimal(100, 1000),
                    Duration = faker.Random.Int(1, 10),
                    EmployeeId = employees[faker.Random.Int(0, employees.Count - 1)].EmployeeId,
                    Status = "Active"
                };

                dbContext.ServicePrices.Add(service);
                services.Add(service);
            }

            await dbContext.SaveChangesAsync();
            return services;
        }

        private static async Task SeedOrdersAsync(DiamondAssessmentDbContext dbContext, List<Customer> customers, List<ServicePrice> services)
        {
            foreach (var customer in customers)
            {
                var numOrders = faker.Random.Int(1, 2);
                for (int i = 0; i < numOrders; i++)
                {
                    var service = faker.PickRandom(services);
                    var order = new Order
                    {
                        CustomerId = customer.CustomerId,
                        ServiceId = service.ServiceId,
                        OrderDate = faker.Date.Past(),
                        TotalPrice = service.Price,
                        Status = "Paid",
                    };

                    dbContext.Orders.Add(order);

                    // Add Payment
                    var payment = new Payment
                    {
                        Order = order,
                        PaymentDate = order.OrderDate.AddDays(1),
                        Amount = order.TotalPrice,
                        Method = "Credit Card",
                        Status = "Completed"
                    };

                    dbContext.Payments.Add(payment);
                }
            }
            await dbContext.SaveChangesAsync();
        }

        private static async Task<List<Request>> SeedRequestsAsync(DiamondAssessmentDbContext dbContext, List<Customer> customers, List<Employee> employees, List<ServicePrice> services)
        {
            var requests = new List<Request>();

            foreach (var customer in customers)
            {
                var numRequests = faker.Random.Int(1, 2);
                for (int i = 0; i < numRequests; i++)
                {
                    var service = faker.PickRandom(services);
                    var employee = faker.PickRandom(employees);

                    var request = new Request
                    {
                        CustomerId = customer.CustomerId,
                        ServiceId = service.ServiceId,
                        EmployeeId = employee.EmployeeId,
                        RequestDate = faker.Date.Past(),
                        RequestType = "Assessment",
                        Status = "Completed"
                    };

                    dbContext.Requests.Add(request);
                    requests.Add(request);
                }
            }

            await dbContext.SaveChangesAsync();
            return requests;
        }

        private static async Task SeedResultsAndCertificatesAsync(DiamondAssessmentDbContext dbContext, List<Request> requests, List<Employee> employees)
        {
            foreach (var request in requests)
            {
                var result = new Result
                {
                    RequestId = request.RequestId,
                    DiamondId = faker.Random.Int(1000, 9999),
                    DiamondOrigin = faker.Address.Country(),
                    Shape = faker.PickRandom(new[] { "Round", "Princess", "Oval" }),
                    Measurements = $"{faker.Random.Double(4, 10):0.00} x {faker.Random.Double(4, 10):0.00} x {faker.Random.Double(2, 6):0.00}",
                    CaratWeight = faker.Random.Decimal(0.5m, 5.0m),
                    Color = faker.PickRandom(new[] { "D", "E", "F", "G" }),
                    Clarity = faker.PickRandom(new[] { "IF", "VVS1", "VVS2", "VS1", "VS2" }),
                    Cut = faker.PickRandom(new[] { "Excellent", "Very Good", "Good" }),
                    Status = "Valid"
                };

                dbContext.Results.Add(result);

                // Certificate
                var certificate = new Certificate
                {
                    Result = result,
                    CertificateNumber = faker.Random.AlphaNumeric(8),
                    IssueDate = faker.Date.Past(),
                    ApprovedBy = faker.PickRandom(employees).EmployeeId,
                    ApprovedDate = DateTime.Now,
                    Status = "Approved"
                };

                dbContext.Certificates.Add(certificate);
            }

            await dbContext.SaveChangesAsync();
        }

        private static async Task SeedConversationsAndChatsAsync(DiamondAssessmentDbContext dbContext, List<Customer> customers, List<Employee> employees)
        {
            foreach (var customer in customers)
            {
                var employee = faker.PickRandom(employees);

                var conversation = new Conversation
                {
                    CustomerId = customer.CustomerId,
                    EmployeeId = employee.EmployeeId,
                    CreatedAt = DateTime.Now,
                    Status = "open"
                };

                dbContext.Conversations.Add(conversation);

                for (int i = 0; i < faker.Random.Int(3, 6); i++)
                {
                    var fromCustomer = i % 2 == 0;

                    var chat = new ChatLog
                    {
                        Conversation = conversation,
                        SenderId = fromCustomer ? customer.CustomerId : employee.EmployeeId,
                        SenderName = fromCustomer ? "Customer" : "Employee",
                        SenderRole = fromCustomer ? "Customer" : "Consultant",
                        MessageType = Enums.MessageType.Text,
                        Message = faker.Lorem.Sentence(),
                        IsRead = false,
                        SentAt = DateTime.Now
                    };

                    dbContext.ChatLogs.Add(chat);
                }
            }

            await dbContext.SaveChangesAsync();
        }
    }
}
