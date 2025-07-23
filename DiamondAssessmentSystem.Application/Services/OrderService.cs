using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using DiamondAssessmentSystem.Infrastructure.Repository;

namespace DiamondAssessmentSystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepo;
        private readonly IRequestRepository _requestRepo;
        private readonly IPaymentRepository _paymentRepo;
        private readonly ICurrentUserService _currentUser;
        private readonly IVnPayService _vnPay;
        private readonly IServicePriceRepository _servicePriceRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly IRequestService _requestService;

        public OrderService(
            IOrderRepository orderRepo,
            IMapper mapper,
            ICurrentUserService currentUser,
            IRequestRepository requestRepo,
            IVnPayService vnPay,
            IPaymentRepository paymentRepo,
            IServicePriceRepository servicePriceRepository,
            ICustomerRepository customerRepository,
            IRequestService requestService)
        {
            _orderRepo = orderRepo;
            _mapper = mapper;
            _currentUser = currentUser;
            _requestRepo = requestRepo;
            _vnPay = vnPay;
            _paymentRepo = paymentRepo;
            _servicePriceRepository = servicePriceRepository;
            _customerRepository = customerRepository;
            _requestService = requestService;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync() =>
            _mapper.Map<IEnumerable<OrderDto>>(await _orderRepo.GetOrdersAsync());

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepo.GetOrderByIdAsync(id);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string userId) =>
            _mapper.Map<IEnumerable<OrderDto>>(await _orderRepo.GetOrdersByCustomerAsync(userId));

        public async Task<bool> CreateOrderAsync(string userId, int requestId, OrderCreateDto dto, string paymentType, VnPaymentResponseFromFe? paymentRequest)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (!await _requestService.UpdateRequestStatusAsync(requestId, "Ordered")) return false;

            var order = _mapper.Map<Order>(dto);

            switch (paymentType)
            {
                case "Online":
                    var result = _vnPay.ExecutePayment(paymentRequest!);
                    if (result == null || !result.Success) return false;
                    order.Status = "Completed";
                    break;

                case "Offline":
                    order.Status = "Pending";
                    break;

                default:
                    throw new ArgumentException($"Invalid payment type: {paymentType}");
            }

            if (!await _orderRepo.CreateOrderAsync(userId, order))
                throw new InvalidOperationException("Failed to create order.");

            await HandlePaymentRecordAsync(order.OrderId, order.TotalPrice, paymentType, order.Status);

            return true;
        }

        private async Task HandlePaymentRecordAsync(int orderId, decimal amount, string method, string status)
        {
            var payment = await _paymentRepo.GetPaymentByOrderId(orderId);
            if (payment == null)
            {
                var newPayment = new Payment
                {
                    OrderId = orderId,
                    Amount = amount,
                    Method = method,
                    Status = status,
                    PaymentDate = DateTime.UtcNow
                };
                await _paymentRepo.CreatePayment(newPayment);
            }
            else
            {
                payment.Method = method;
                payment.Status = status;
                payment.PaymentDate = DateTime.UtcNow;
                await _paymentRepo.UpdatePayment(payment);
            }
        }

        public async Task<bool> CancelOrderAsync(int id) =>
            await _orderRepo.CancelOrderAsync(id);
        public async Task<bool> UpdatePaymentAsync(string userId, int orderId, string status)
        {
            var order = await _orderRepo.GetOrderByIdAsync(orderId);
            if (status == "Completed")
            {
                order.Status = "Completed";
                await _orderRepo.UpdateOrderAsync(order);
            }

            if (order?.Customer.UserId != userId) return false;

            return await HandleOfflinePaymentUpdateAsync(order, status);
        }

        private async Task<bool> HandleOfflinePaymentUpdateAsync(Order order, string status)
        {
            var payment = await _paymentRepo.GetPaymentByOrderId(order.OrderId);
            if (payment == null)
            {
                var newPayment = new Payment
                {
                    OrderId = order.OrderId,
                    Amount = order.TotalPrice,
                    Method = "Offline",
                    Status = status,
                    PaymentDate = DateTime.UtcNow
                };
                return await _paymentRepo.CreatePayment(newPayment);
            }

            payment.Status = status;
            payment.Method = "Offline";
            payment.PaymentDate = DateTime.UtcNow;
            return await _paymentRepo.UpdatePayment(payment);
        }

        private async Task<bool> UpdateRequestStatusAsync(int requestId)
        {
            var request = await _requestRepo.GetRequestByIdAsync(requestId);
            if (request == null) return false;

            request.Status = "Pending";
            return await _requestRepo.UpdateRequestAsync(request);
        }
    }
}
