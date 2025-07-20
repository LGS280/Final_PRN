using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Application.Map;
using DiamondAssessmentSystem.Application.Services;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using DiamondAssessmentSystem.Infrastructure.Repository;
using DiamondAssessmentSystem.Infrastructure.SeedData;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==================== AutoMapper ====================
            builder.Services.AddAutoMapper(typeof(MapProfile));

            // ==================== Controllers ====================
            builder.Services.AddControllers();

            // ==================== DbContext ====================
            builder.Services.AddDbContext<DiamondAssessmentDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // ==================== HttpContextAccessor ====================
            builder.Services.AddHttpContextAccessor();

            // ==================== Identity ====================
            builder.Services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DiamondAssessmentDbContext>()
                .AddDefaultTokenProviders();

            // Optional cookie paths (good for future UI)
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AllUsersPolicy", policy =>
                {
                    policy.RequireRole("User","Admin");
                    policy.RequireRole("User", "Employee");
                    policy.RequireRole("User", "Customer");
                    policy.RequireRole("User", "Consultant");
                });
                //options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
                //options.AddPolicy("CustomerOnly", policy => policy.RequireRole("Customer"));
                //options.AddPolicy("ConsultantOnly", policy => policy.RequireRole("Consultant"));
            });

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                  .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                  {
                      options.Cookie.Name = "CookieAuth"; //Important for code with authentication!
                      options.LoginPath = "/Auth/Login";  // Specify the login path
                      //options.AccessDeniedPath = "/Auth/AccessDenied"; // Specify where to redirect for unauthorized access
                      options.LogoutPath = "/Auth/Logout";
                  });

            // ==================== Application Services ====================
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IServicePriceService, ServicePriceService>();
            builder.Services.AddScoped<IResultService, ResultService>();
            builder.Services.AddScoped<IRequestService, RequestService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<ICerterficateService, CertificateService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IVnPayService, VnPayService>();
            builder.Services.AddScoped<IBlogService, BlogService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
            builder.Services.AddScoped<IConversationService, ConversationService>();
            builder.Services.AddScoped<IChatMessageService, ChatMessageService>();

            // ==================== Repositories ====================
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IBlogRepository, BlogRepository>();
            builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();
            builder.Services.AddScoped<IResultRepository, ResultRepository>();
            builder.Services.AddScoped<IServicePriceRepository, ServicePriceRepository>();
            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IRequestRepository, RequestRepository>();
            builder.Services.AddScoped<IReportRepository, ReportRepository>();
            builder.Services.AddScoped<IConversationRepository, ConversationRepository>();
            builder.Services.AddScoped<IChatLogRepository, ChatLogRepository>();

            // ==================== SignalR ====================
            builder.Services.AddSignalR();

            // ==================== JWT Authentication ====================
            var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var tokenFromCookie = context.Request.Cookies["access_token"];
                        if (!string.IsNullOrEmpty(tokenFromCookie))
                        {
                            context.Token = tokenFromCookie;
                        }

                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub/chat"))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            builder.Services.AddAuthorization();


            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            // ==================== Database seeding ====================
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var context = services.GetRequiredService<DiamondAssessmentDbContext>();

                await DbInitializer.SeedDefaultAdminAsync(services);
                await RoleSeeder.SeedRolesAsync(roleManager, logger);
                await DataSeeder.SeedSampleDataAsync(services, context);
            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
