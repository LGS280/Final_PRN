using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUser;
        private readonly IRequestService _requestService;
        private readonly IServicePriceService _servicePriceService;

        public OrderController(
            IOrderService orderService,
            ICurrentUserService currentUser,
            IRequestService requestService,
            IServicePriceService servicePriceService)
        {
            _orderService = orderService;
            _currentUser = currentUser;
            _requestService = requestService;
            _servicePriceService = servicePriceService;
        }

        // GET: /Order/All (Admin/Staff)
        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetOrdersAsync();
            return View(orders);
        }

        // GET: /Order/MyOrder (User)
        public async Task<IActionResult> MyOrder()
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var orders = await _orderService.GetOrdersByCustomerAsync(userId);
            return View(orders);
        }

        // GET: /Order/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound();

            return View(order);
        }

        // GET: /Order/Create
        public async Task<IActionResult> Create()
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var requestWithServices = await _requestService.GetDraftOrPendingRequestsWithServiceAsync(userId);

            // Build dropdown list
            ViewBag.RequestOptions = requestWithServices.Select(r => new SelectListItem
            {
                Value = r.RequestId.ToString(),
                Text = $"Request #{r.RequestId} - {r.RequestType} ({r.ServiceType})"
            }).ToList();

            // Build JS mapping: RequestId => { serviceId, price }
            var serviceMap = requestWithServices.ToDictionary(
                r => r.RequestId.ToString(),
                r => new
                {
                    serviceId = r.ServiceId,
                    price = r.Price
                });

            ViewBag.ServiceMapJson = JsonConvert.SerializeObject(
                serviceMap,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

            return View(new OrderCreateCombineDto());
        }

        // POST: /Order/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateCombineDto model)
        {
            var userId = _currentUser.UserId;
            if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                await LoadRequestOptionsToViewBag(userId);
                return View(model);
            }

            try
            {
                // Nếu là Online → Redirect sang PaymentController
                if (model.PaymentInfo.PaymentType == "Online")
                {
                    var paymentRequest = new VnPaymentRequestDto
                    {
                        RequestId = model.PaymentInfo.RequestId,
                        ServiceId = model.OrderData.ServiceId,
                        Amount = (double)model.OrderData.TotalPrice,
                        CreatedDate = DateTime.UtcNow
                    };

                    return RedirectToAction("RedirectToVnPay", "Payment", paymentRequest);
                }

                // Nếu là Offline → Tạo luôn
                var created = await _orderService.CreateOrderAsync(
                    userId,
                    model.PaymentInfo.RequestId,
                    model.OrderData,
                    "Offline",
                    null // Không có paymentRequest
                );

                if (!created)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create order.");
                    await LoadRequestOptionsToViewBag(userId);
                    return View(model);
                }

                return RedirectToAction(nameof(MyOrder));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                await LoadRequestOptionsToViewBag(userId);
                return View(model);
            }
        }

        //// GET: /Order/Edit/5
        //public async Task<IActionResult> Edit(int id)
        //{
        //    var userId = _currentUser.UserId;
        //    if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Auth");

        //    var order = await _orderService.GetOrderByIdAsync(id);
        //    if (order == null || order.Status == "Completed" || order.Status == "Canceled")
        //        return NotFound();

        //    var requests = await _requestService.GetDraftOrPendingRequestsWithServiceAsync(userId);

        //    ViewBag.RequestOptions = requests.Select(r => new SelectListItem
        //    {
        //        Value = r.RequestId.ToString(),
        //        Text = $"Request #{r.RequestId} - {r.RequestType} ({r.ServiceType})"
        //    }).ToList();

        //    var serviceMap = requests.ToDictionary(
        //        r => r.RequestId.ToString(),
        //        r => new { serviceId = r.ServiceId, price = r.Price });

        //    ViewBag.ServiceMapJson = JsonConvert.SerializeObject(
        //        serviceMap,
        //        new JsonSerializerSettings
        //        {
        //            ContractResolver = new CamelCasePropertyNamesContractResolver(),
        //            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //        });

        //    // Ghép dữ liệu vào CombineDto
        //    var model = new OrderCreateCombineDto
        //    {
        //        OrderData = new OrderCreateDto
        //        {
        //            OrderDate = order.OrderDate,
        //            ServiceId = order.ServiceId,
        //            TotalPrice = order.TotalPrice
        //        },
        //        PaymentInfo = new OrderPaymentDto
        //        {
        //            RequestId = order.RequestId,
        //            PaymentType = "Offline" 
        //        }
        //    };

        //    ViewBag.OrderId = order.OrderId;
        //    return View(model);
        //}

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, OrderCreateCombineDto model)
        //{
        //    var userId = _currentUser.UserId;
        //    if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login", "Auth");

        //    if (!ModelState.IsValid)
        //    {
        //        await LoadRequestOptionsToViewBag(userId);
        //        return View(model);
        //    }

        //    var success = await _orderService.UpdateOrderAsync(
        //        id, userId,
        //        model.OrderData,
        //        model.PaymentInfo.RequestId,
        //        model.PaymentInfo.PaymentType
        //    );

        //    if (!success)
        //    {
        //        ModelState.AddModelError(string.Empty, "Failed to update order.");
        //        await LoadRequestOptionsToViewBag(userId);
        //        return View(model);
        //    }

        //    return RedirectToAction(nameof(MyOrder));
        //}

        // POST: /Order/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _currentUser.UserId;
            if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var success = await _orderService.CancelOrderAsync(id);
            if (!success)
                ModelState.AddModelError(string.Empty, "Cancel failed.");

            return RedirectToAction(nameof(MyOrder));
        }

        // Load lại request list nếu form có lỗi
        private async Task LoadRequestOptionsToViewBag(string userId)
        {
            var requests = await _requestService.GetDraftOrPendingRequestsAsync(userId);
            ViewBag.RequestOptions = requests.Select(r => new SelectListItem
            {
                Value = r.RequestId.ToString(),
                Text = $"Request #{r.RequestId} - {r.RequestType}"
            }).ToList();
        }
    }
}
