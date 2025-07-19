using DiamondAssessmentSystem.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetOrdersAsync();
        Task<OrderDto?> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string userId);
        Task<bool> CreateOrderAsync(
            string userId,
            int requestId,
            OrderCreateDto orderCreateDto,
            string paymentType,
            VnPaymentResponseFromFe paymentRequest);
        //Task<bool> UpdateOrderAsync(int orderId, string userId, OrderCreateDto data, int requestId, string paymentType);
        Task<bool> CancelOrderAsync(int id);
        Task<bool> UpdatePaymentAsync(string userId, int orderId, string status);
    }
}
