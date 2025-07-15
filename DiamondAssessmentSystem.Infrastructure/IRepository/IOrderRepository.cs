using DiamondAssessmentSystem.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string userId);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<bool> CreateOrderAsync(string userId, Order order);
        Task<bool> UpdateOrderAsync(Order order);
        Task<bool> CancelOrderAsync(int id);
    }
}
