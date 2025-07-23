using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    [Authorize(Roles = "Staff,Customer, Manager")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICurrentUserService _currentUser;
        private readonly IRequestService _requestService;
        private readonly IServicePriceService _servicePriceService;
        private readonly ICustomerService _customerService;

        public OrderController(
            IOrderService orderService,
            ICurrentUserService currentUser,
            IRequestService requestService,
            IServicePriceService servicePriceService,
            ICustomerService customerService)
        {
            _orderService = orderService;
            _currentUser = currentUser;
            _requestService = requestService;
            _servicePriceService = servicePriceService;
            _customerService = customerService;
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

        [HttpGet]
        public async Task<IActionResult> Create(int? requestId)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var customer = await _customerService.GetCustomerByIdAsync(userId);
            ViewBag.CustomerInfo = customer;

            var requests = await _requestService.GetDraftOrPendingRequestsWithServiceAsync(userId);

            ViewBag.RequestOptions = requests.Select(r => new SelectListItem
            {
                Value = r.RequestId.ToString(),
                Text = $"Request #{r.RequestId} - {r.RequestType} ({r.ServiceType})"
            }).ToList();

            var map = requests.ToDictionary(
                r => r.RequestId.ToString(),
                r => new
                {
                    requestType = r.RequestType,
                    requestDate = r.RequestDate.ToString("yyyy-MM-dd"),
                    status = r.Status,
                    serviceId = r.ServiceId,
                    serviceType = r.ServiceType,
                    price = r.Price,
                    duration = r.Duration,
                    description = r.Description,
                    employeeName = r.EmployeeName
                });

            ViewBag.ServiceMapJson = JsonConvert.SerializeObject(map, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            if (requestId.HasValue && map.TryGetValue(requestId.Value.ToString(), out var service))
            {
                var dto = new OrderCreateCombineDto
                {
                    PaymentInfo = new OrderPaymentDto
                    {
                        RequestId = requestId.Value,
                        PaymentType = ""
                    },
                    OrderData = new OrderCreateDto
                    {
                        OrderDate = DateTime.UtcNow,
                        ServiceId = service.serviceId,
                        TotalPrice = Convert.ToDecimal(service.price)
                    }
                };
                return View(dto);
            }

            return View(new OrderCreateCombineDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateCombineDto model)
        {
            var userId = _currentUser.UserId;
            if (!User.Identity.IsAuthenticated || string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var customer = await _customerService.GetCustomerByIdAsync(userId);
            if (IsCustomerProfileIncomplete(customer))
            {
                TempData["ProfileIncomplete"] = true;
                await LoadRequestOptionsToViewBag(userId);
                ViewBag.CustomerInfo = customer;
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                await LoadRequestOptionsToViewBag(userId);
                ViewBag.CustomerInfo = customer;
                return View(model);
            }

            try
            {
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

                var created = await _orderService.CreateOrderAsync(
                    userId,
                    model.PaymentInfo.RequestId,
                    model.OrderData,
                    "Offline",
                    null
                );

                if (!created)
                {
                    ModelState.AddModelError(string.Empty, "Failed to create order.");
                    await LoadRequestOptionsToViewBag(userId);
                    ViewBag.CustomerInfo = customer;
                    return View(model);
                }

                return _currentUser.Role?.ToLower() == "customer"
                    ? RedirectToAction(nameof(MyOrder))
                    : RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
                await LoadRequestOptionsToViewBag(userId);
                ViewBag.CustomerInfo = customer;
                return View(model);
            }
        }

        private async Task LoadRequestOptionsToViewBag(string userId)
        {
            var requests = await _requestService.GetDraftOrPendingRequestsWithServiceAsync(userId);

            ViewBag.RequestOptions = requests.Select(r => new SelectListItem
            {
                Value = r.RequestId.ToString(),
                Text = $"Request #{r.RequestId} - {r.RequestType} ({r.ServiceType})"
            }).ToList();

            var map = requests.ToDictionary(
                r => r.RequestId.ToString(),
                r => new
                {
                    requestType = r.RequestType,
                    requestDate = r.RequestDate.ToString("yyyy-MM-dd"),
                    status = r.Status,
                    serviceId = r.ServiceId,
                    serviceType = r.ServiceType,
                    price = r.Price,
                    duration = r.Duration,
                    description = r.Description,
                    employeeName = r.EmployeeName
                });

            ViewBag.ServiceMapJson = JsonConvert.SerializeObject(map, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
        }

        private bool IsCustomerProfileIncomplete(CustomerDto c)
        {
            return string.IsNullOrWhiteSpace(c.FirstName) ||
                   string.IsNullOrWhiteSpace(c.LastName) ||
                   string.IsNullOrWhiteSpace(c.Gender) ||
                   string.IsNullOrWhiteSpace(c.Phone) ||
                   string.IsNullOrWhiteSpace(c.Email) ||
                   string.IsNullOrWhiteSpace(c.IdCard) ||
                   string.IsNullOrWhiteSpace(c.Address) ||
                   string.IsNullOrWhiteSpace(c.UnitName) ||
                   string.IsNullOrWhiteSpace(c.TaxCode);
        }

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
    }
}
