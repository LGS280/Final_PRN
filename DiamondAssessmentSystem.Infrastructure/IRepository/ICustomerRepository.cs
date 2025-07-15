using DiamondAssessmentSystem.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface ICustomerRepository
    {
        Task<int> GetCustomerIdAsync(string userId);
        Task<Customer?> GetCustomerByIdAsync(string userId);
        Task<bool> UpdateCustomerAsync(Customer customer);
    }
}
