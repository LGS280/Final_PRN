using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public PaymentRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<Payment?> GetPaymentByOrderId(int orderId)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.OrderId == orderId);
        }

        public async Task<bool> CreatePayment(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePayment(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
