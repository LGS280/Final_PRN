using DiamondAssessmentSystem.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface IServicePriceRepository
    {
        Task<IEnumerable<ServicePrice>> GetAllAsync();
        Task<IEnumerable<ServicePrice>> GetByStatusAsync(string status);
        Task<ServicePrice?> GetByIdAsync(int id);
        Task<ServicePrice> AddAsync(ServicePrice servicePrice);
        Task<bool> UpdateAsync(ServicePrice servicePrice);
    }
}
