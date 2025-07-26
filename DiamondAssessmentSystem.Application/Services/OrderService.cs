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
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IOrderRepository orderRepo,
            IMapper mapper,
            ICurrentUserService currentUser,
            IRequestRepository requestRepo,
            IVnPayService vnPay,
            IPaymentRepository paymentRepo,
            IServicePriceRepository servicePriceRepository,
            ICustomerRepository customerRepository,
            IRequestService requestService,
            IUnitOfWork unitOfWork)
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
            _unitOfWork = unitOfWork;
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

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var order = _mapper.Map<Order>(dto);
                order.Status = "Pending";

                if (!await _orderRepo.CreateOrderAsync(userId, order))
                    throw new InvalidOperationException("Failed to create order.");

                if (paymentType == "Online")
                {
                    var paymentResult = _vnPay.ExecutePayment(paymentRequest!);
                    if (paymentResult == null || !paymentResult.Success)
                        throw new InvalidOperationException("VNPAY payment failed.");

                    if (!await CreatePaymentRecordAsync(order.OrderId, order.TotalPrice, "Online", "Completed"))
                        throw new InvalidOperationException("Failed to save payment.");

                    order.Status = "Completed";
                    order.OrderDate = DateTime.Now;
                    await _orderRepo.UpdateOrderAsync(order);
                }
                else if (paymentType == "Offline")
                {
                    order.OrderDate = DateTime.Now;
                    if (!await CreatePaymentRecordAsync(order.OrderId, order.TotalPrice, "Offline", "Pending"))
                        throw new InvalidOperationException("Failed to save offline payment.");
                }
                else
                {
                    throw new ArgumentException($"Invalid payment type: {paymentType}");
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task<bool> CreatePaymentRecordAsync(int orderId, decimal amount, string method, string status)
        {
            var payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Method = method,
                Status = status.ToString(),
                PaymentDate = DateTime.UtcNow
            };
            return await _paymentRepo.CreatePayment(payment);
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
            if (order == null || order.Customer.UserId != userId) return false;

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (status == "Completed")
                {
                    order.Status = "Completed";
                    await _orderRepo.UpdateOrderAsync(order);
                }

                var payment = await _paymentRepo.GetPaymentByOrderId(orderId);
                if (payment != null)
                {
                    payment.Status = "Completed";
                    payment.Method = "Offline";
                    payment.PaymentDate = DateTime.UtcNow;
                    await _paymentRepo.UpdatePayment(payment);
                }

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
