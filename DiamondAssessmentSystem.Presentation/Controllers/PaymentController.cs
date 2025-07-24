using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IVnPayService _vnPayService;
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUser;

        public PaymentController(
            IVnPayService vnPayService,
            IOrderService orderService,
            ICurrentUserService currentUser)
        {
            _vnPayService = vnPayService;
            _orderService = orderService;
            _currentUser = currentUser;
        }

        // GET: /Payment/RedirectToVnPay
        public async Task<IActionResult> RedirectToVnPay(VnPaymentRequestDto paymentRequest)
        {
            if (paymentRequest == null || paymentRequest.Amount <= 0 || paymentRequest.RequestId <= 0)
                return RedirectToAction("Create", "Order");

            var url = await _vnPayService.CreatePatmentUrl(HttpContext, paymentRequest);
            return Redirect(url);
        }

        // GET: /Payment/Confirm
        [AllowAnonymous]
        public async Task<IActionResult> Confirm(VnPaymentResponseFromFe response)
        {
            if (response == null)
                return RedirectToAction("PaymentFailed");

            var result = _vnPayService.ExecutePayment(response);
            if (!result.Success)
                return RedirectToAction("PaymentFailed");

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            // Parse OrderInfo
            var orderMeta = ParseOrderInfo(response.OrderInfo);
            if (!orderMeta.TryGetValue("serviceId", out var serviceIdStr) ||
                !orderMeta.TryGetValue("amount", out var amountStr) ||
                !orderMeta.TryGetValue("requestId", out var requestIdStr))
            {
                return RedirectToAction("PaymentFailed");
            }

            var orderDto = new OrderCreateDto
            {
                OrderDate = DateTime.UtcNow,
                ServiceId = int.Parse(serviceIdStr),
                TotalPrice = Convert.ToDecimal(amountStr)
            };

            var created = await _orderService.CreateOrderAsync(
                userId,
                int.Parse(requestIdStr),
                orderDto,
                "Online",
                response);

            if (!created)
                return RedirectToAction("PaymentFailed");

            return RedirectToAction("MyOrder", "Order");
        }

        // GET: /Payment/Failed
        public IActionResult PaymentFailed()
        {
            return View(); // Views/Payment/PaymentFailed.cshtml
        }

        // Helper: Parse OrderInfo (e.g. requestId=5;serviceId=2;amount=300000)
        private Dictionary<string, string> ParseOrderInfo(string orderInfo)
        {
            var dict = new Dictionary<string, string>();
            if (string.IsNullOrWhiteSpace(orderInfo)) return dict;

            var parts = orderInfo.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var keyValue = part.Split('=');
                if (keyValue.Length == 2)
                {
                    dict[keyValue[0].Trim()] = keyValue[1].Trim();
                }
            }

            return dict;
        }
    }
}
