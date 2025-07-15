using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IVnPayService _vnPayService;
        private readonly IMapper _mapper;

        public OrderService(
            IOrderRepository orderRepository,
            IMapper mapper,
            ICurrentUserService currentUser,
            IRequestRepository requestRepository,
            IVnPayService vnPayService,
            IPaymentRepository paymentRepository)
        {
            _orderRepository = orderRepository;
            _currentUser = currentUser;
            _mapper = mapper;
            _requestRepository = requestRepository;
            _vnPayService = vnPayService;
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersAsync()
        {
            var orders = await _orderRepository.GetOrdersAsync();
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            return order == null ? null : _mapper.Map<OrderDto>(order);
        }

        public async Task<IEnumerable<OrderDto>> GetOrdersByCustomerAsync(string userId)
        {
            var orders = await _orderRepository.GetOrdersByCustomerAsync(userId);
            return _mapper.Map<IEnumerable<OrderDto>>(orders);
        }

        public async Task<bool> CreateOrderAsync(
            string userId,
            int requestId,
            OrderCreateDto orderCreateDto,
            string paymentType,
            VnPaymentResponseFromFe paymentRequest)
        {
            if (!await UpdateRequestStatusAsync(requestId))
                return false;

            if (orderCreateDto == null)
                throw new ArgumentNullException(nameof(orderCreateDto));

            var order = _mapper.Map<Order>(orderCreateDto);

            if (paymentType == "Online")
            {
                var paymentResult = _vnPayService.ExecutePayment(paymentRequest);
                if (paymentResult == null || !paymentResult.Success)
                    return false;

                order.Status = "Completed";
            }
            else if (paymentType == "Offline")
            {
                order.Status = "Pending";
            }
            else
            {
                throw new ArgumentException($"Unsupported payment type: {paymentType}");
            }

            var created = await _orderRepository.CreateOrderAsync(userId, order);
            if (!created)
                throw new InvalidOperationException("Problem creating order.");

            var payment = await _paymentRepository.GetPaymentByOrderId(order.OrderId);
            if (payment == null)
            {
                var newPayment = new Payment
                {
                    OrderId = order.OrderId,
                    PaymentDate = DateTime.UtcNow,
                    Amount = order.TotalPrice,
                    Method = paymentType,
                    Status = order.Status
                };
                await _paymentRepository.CreatePayment(newPayment);
            }
            else
            {
                payment.Method = paymentType;
                payment.Status = order.Status;
                await _paymentRepository.UpdatePayment(payment);
            }

            return true;
        }

        public async Task<bool> UpdateOrderAsync(int id, OrderCreateDto orderCreateDto)
        {
            var existing = await _orderRepository.GetOrderByIdAsync(id);
            if (existing == null)
                return false;

            if (existing.Status == "Completed" || existing.Status == "Canceled")
            {
                return false; 
            }

            _mapper.Map(orderCreateDto, existing);
            return await _orderRepository.UpdateOrderAsync(existing);
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            return await _orderRepository.CancelOrderAsync(id);
        }

        public async Task<bool> UpdatePaymentAsync(string userId, int orderId, string status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null || order.Customer.UserId != userId)
            {
                return false;
            }

            var payment = await _paymentRepository.GetPaymentByOrderId(orderId);
            if (payment == null)
            {
                var newPayment = new Payment
                {
                    OrderId = orderId,
                    PaymentDate = DateTime.UtcNow,
                    Amount = order.TotalPrice,
                    Method = "Offline",
                    Status = status
                };
                return await _paymentRepository.CreatePayment(newPayment);
            }
            else
            {
                payment.Status = status;
                payment.Method = "Offline";
                payment.PaymentDate = DateTime.UtcNow;
                return await _paymentRepository.UpdatePayment(payment);
            }
        }

        private async Task<bool> UpdateRequestStatusAsync(int requestId)
        {
            var request = await _requestRepository.GetRequestByIdAsync(requestId);
            if (request == null)
                return false;

            request.Status = "Pending";
            return await _requestRepository.UpdateRequestAsync(request);
        }
    }
}
