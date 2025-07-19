using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public OrderRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Payments)
                .Include(o => o.Service)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetOrdersByCustomerAsync(string userId)
        {
            var customerId = await GetCustomerIdAsync(userId);
            if (customerId == -1)
                return Enumerable.Empty<Order>();

            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.Customer)
                .Include(o => o.Payments)
                .Include(o => o.Service)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Payments)
                .Include(o => o.Service)
                .FirstOrDefaultAsync(o => o.OrderId == id);
        }

        public async Task<bool> CreateOrderAsync(string userId, Order order)
        {
            var customerId = await GetCustomerIdAsync(userId);
            if (customerId == -1)
                return false;

            order.CustomerId = customerId;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Order?> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await OrderExistsAsync(order.OrderId))
                    return false;
                throw;
            }
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                return false;

            order.Status = "Canceled";
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<int> GetCustomerIdAsync(string userId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == userId);
            return customer?.CustomerId ?? -1;
        }

        private async Task<bool> OrderExistsAsync(int id)
        {
            return await _context.Orders.AnyAsync(e => e.OrderId == id);
        }
    }
}
