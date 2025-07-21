using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface IUserRepository
    {
        Task<IdentityResult> CreateEmployeeWithRoleAsync(User user, string password, string role);
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> UserExistsAsync(string username);
        Task<IdentityResult> RegisterCustomerAsync(User user, string password, string email);
        Task<User?> LoginAsync(string usernameOrEmail, string password);
        Task<User?> ValidateUserCredentialsAsync(string email, string password);
        Task<IList<string>> GetUserRolesAsync(User user);
        Task<int?> GetAssociatedIdByUserIdAsync(string userId);

    }
}
