using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DiamondAssessmentDbContext _context;

        public UserRepository(UserManager<User> userManager,
                                 RoleManager<IdentityRole> roleManager,
                                 DiamondAssessmentDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<IdentityResult> CreateEmployeeWithRoleAsync(User user, string password, string role)
        {

            // Tạo user
            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                return createResult;

            // Tạo role nếu chưa có
            if (!await _roleManager.RoleExistsAsync(role))
            {
                var roleCreateResult = await _roleManager.CreateAsync(new IdentityRole(role));
                if (!roleCreateResult.Succeeded)
                    return IdentityResult.Failed(roleCreateResult.Errors.ToArray());
            }

            // Gán role cho user
            var roleResult = await _userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded)
                return roleResult;

            // Ghi vào bảng Employees nếu đúng UserType
            if (user.UserType == "Employee")
            {
                _context.Employees.Add(new Employee
                {
                    UserId = user.Id
                });

                await _context.SaveChangesAsync();
            }

            return IdentityResult.Success;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userManager.Users
                .Include(u => u.Customer)
                .Include(u => u.Employee)
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string userId)
        {
            return await _userManager.Users
                .Include(u => u.Customer)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return false;

            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.UserId == userId);
            if (customer != null) _context.Customers.Remove(customer);

            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.UserId == userId);
            if (employee != null) _context.Employees.Remove(employee);

            await _context.SaveChangesAsync();

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            var existingUser = await _userManager.FindByIdAsync(user.Id);
            if (existingUser == null) return false;

            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Email = user.Email;
            existingUser.Gender = user.Gender;
            existingUser.Status = user.Status;
            existingUser.Note = user.Note;

            var result = await _userManager.UpdateAsync(existingUser);
            return result.Succeeded;
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _userManager.FindByNameAsync(username) != null;
        }

        public async Task<IdentityResult> RegisterCustomerAsync(User user, string password, string email)
        {
            if (await _userManager.FindByEmailAsync(email) != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email is already in use." });
            }

            user.Email = email;

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return result;

            if (!await _roleManager.RoleExistsAsync("Customer"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            }

            await _userManager.AddToRoleAsync(user, "Customer");

            var customer = new Customer
            {
                UserId = user.Id,
                UnitName = "Null"
            };

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return IdentityResult.Success;
        }

        public async Task<User?> ValidateUserCredentialsAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
            {
                return user;
            }
            return null;
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<User?> LoginAsync(string usernameOrEmail, string password)
        {
            var user = await _userManager.Users
                .Include(u => u.Customer)
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.UserName == usernameOrEmail || u.Email == usernameOrEmail);

            if (user == null)
                return null;

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);
            return isPasswordValid ? user : null;
        }

        public async Task<int?> GetAssociatedIdByUserIdAsync(string userId)
        {
            var customerId = await _context.Customers
                .Where(c => c.UserId == userId)
                .Select(c => (int?)c.CustomerId)
                .FirstOrDefaultAsync();

            if (customerId.HasValue)
                return customerId.Value;

            var employeeId = await _context.Employees
                .Where(e => e.UserId == userId)
                .Select(e => (int?)e.EmployeeId)
                .FirstOrDefaultAsync();

            if (employeeId.HasValue)
                return employeeId.Value;

            return null;
        }

        public async Task<User?> GetUserByEmployeeIdAsync(int employeeId)
        {
            return await _context.Users
                .Include(u => u.Employee)
                .FirstOrDefaultAsync(u => u.Employee != null && u.Employee.EmployeeId == employeeId);
        }

    }
}
