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

            var url = await _vnPayService.CreatePaymentUrl(HttpContext, paymentRequest);
            return Redirect(url);
        }

        // GET: /Payment/Confirm
        [HttpGet]
        [AllowAnonymous]
        [Route("Payment/Confirm")]
        public async Task<IActionResult> Confirm([FromQuery] VnPaymentResponseFromFe response)
        {
            if (response == null || string.IsNullOrWhiteSpace(response.VnPayResponseCode))
                return RedirectToAction("PaymentFailed");

            var result = _vnPayService.ExecutePayment(response);
            if (!result.Success || result.OrderId == null)
                return RedirectToAction("PaymentFailed");

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var orderInfo = ParseOrderInfo(response.OrderInfo ?? "");
            if (!orderInfo.TryGetValue("requestId", out var requestIdStr) ||
                !orderInfo.TryGetValue("serviceId", out var serviceIdStr) ||
                !orderInfo.TryGetValue("amount", out var amountStr))
            {
                return RedirectToAction("PaymentFailed");
            }

            var orderDto = new OrderCreateDto
            {
                OrderDate = DateTime.UtcNow,
                ServiceId = int.Parse(serviceIdStr),
                TotalPrice = decimal.Parse(amountStr)
            };

            var created = await _orderService.CreateOrderAsync(
                userId,
                int.Parse(requestIdStr),
                orderDto,
                "Online",
                response);

            if (!created)
                return RedirectToAction("PaymentFailed");

            ViewData["OrderId"] = result.OrderId;
            return View("ConfirmSuccess");
        }

        private string ParseOrderInfoValue(string orderInfo, string key)
        {
            var parts = orderInfo.Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var part in parts)
            {
                var keyValue = part.Split('=');
                if (keyValue.Length == 2 && keyValue[0].Trim() == key)
                    return keyValue[1].Trim();
            }
            return string.Empty;
        }

        // GET: /Payment/Failed
        public IActionResult PaymentFailed()
        {
            TempData["ErrorMessage"] = "Thanh toán thất bại. Vui lòng thử lại hoặc chọn phương thức khác.";
            return View();
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
