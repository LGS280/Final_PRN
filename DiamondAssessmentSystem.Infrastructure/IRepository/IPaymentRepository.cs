using DiamondAssessmentSystem.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface IPaymentRepository
    {
        Task<Payment?> GetPaymentByOrderId(int orderId);
        Task<bool> CreatePayment(Payment payment);
        Task<bool> UpdatePayment(Payment payment);
    }
}
