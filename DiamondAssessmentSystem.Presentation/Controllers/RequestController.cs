using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    [Authorize(Roles = "Staff,Customer,Assessor")]
    public class RequestController : Controller
    {
        private readonly IRequestService _requestService;
        private readonly ICurrentUserService _currentUser;
        private readonly IServicePriceService _servicePriceService;

        public RequestController(
            IRequestService requestService,
            ICurrentUserService currentUser,
            IServicePriceService servicePriceService)
        {
            _requestService = requestService;
            _currentUser = currentUser;
            _servicePriceService = servicePriceService;
        }

        // GET: Request
        public async Task<IActionResult> Index() // Changed from GetAllRequests
        {
            var requests = await _requestService.GetRequestsAsync();
            return View(requests);  // Returns the data
        }

        // GET: Request/Details/{id}
        public async Task<IActionResult> Details(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // GET: Request/My
        public async Task<IActionResult> My() // Changed from GetMyRequests
        {
            if (!User.Identity.IsAuthenticated) //Check to ensure user authentication
            {
                //If unauthenticated direct them to Login
                return RedirectToAction("Login", "Auth");
            }

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                // if there is a userid issue, load to new page/error for the user.
                return View("UnAuthorized");
            }

            var requests = await _requestService.GetRequestsByCustomerIdAsync(userId);

            return View(requests);  //Load and direct data into action.
        }

        // GET: Request/Create
        public async Task<IActionResult> Create()
        {
            var activeServices = await _servicePriceService.GetByStatusAsync("Active");

            ViewBag.Services = activeServices.Select(s => new SelectListItem
            {
                Value = s.ServiceId.ToString(),
                Text = $"{s.ServiceType} - {s.Price:C}" // VD: "Standard - $50.00"
            }).ToList();

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RequestCreateDto createDto, string action)
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            if (!ModelState.IsValid)
            {
                // ⚠️ Cần load lại danh sách dịch vụ nếu có lỗi
                var activeServices = await _servicePriceService.GetByStatusAsync("Active");
                ViewBag.Services = activeServices.Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = $"{s.ServiceType} - {s.Price:C}"
                }).ToList();

                return View("Create", createDto);
            }

            try
            {
                var userId = _currentUser.UserId;
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError(string.Empty, "User is not valid.");
                    return View("Create", createDto);
                }

                string status = action == "Submit" ? "Pending" : "Draft";

                var created = await _requestService.CreateRequestForCustomerAsync(userId, createDto, status);
                return _currentUser.Role == "Customer"
                    ? RedirectToAction(nameof(My))
                    : RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Can’t connect to DB: " + ex.Message);

                var activeServices = await _servicePriceService.GetByStatusAsync("Active");
                ViewBag.Services = activeServices.Select(s => new SelectListItem
                {
                    Value = s.ServiceId.ToString(),
                    Text = $"{s.ServiceType} - {s.Price:C}"
                }).ToList();

                return View("Create", createDto);
            }
        }

        // GET: Request/Cancel/{id}
        [HttpPost, ActionName("Cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            // Validate for user 
            var userId = _currentUser.UserId;

            // Validate action based on request (only to see code can only be done on the right area)
            if (userId == null)
            {
                //Return Error if no User Id, and if the system isn't able to retrieve.
                return RedirectToAction("Error");
            }

            // Try to cancel,
            try
            {
                //Perform action
                var success = await _requestService.CancelRequestAsync(userId, id);
                return RedirectToAction("Index");   //Goes home 
            }
            catch (Exception ex)
            {
                //If system is not, or there is incorrect validation
                ModelState.AddModelError(string.Empty, $"Request could not be validate \n  Error is {ex}");
                return View();  //Sends back code to test, it works!
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            // Ngăn không cho edit nếu status là Pending
            if (request.Status == "Pending")
            {
                TempData["ErrorMessage"] = "Cannot edit a request that is already pending.";
                return RedirectToAction("Details", new { id });
            }

            var requestCreateDto = new RequestCreateDto
            {
                ServiceId = request.ServiceId,
                RequestType = request.RequestType,
                // Nếu có thêm fields thì gán vào đây
            };

            ViewBag.Id = request.RequestId;

            // Load dropdown danh sách dịch vụ
            var activeServices = await _servicePriceService.GetByStatusAsync("Active");
            ViewBag.Services = activeServices.Select(s => new SelectListItem
            {
                Value = s.ServiceId.ToString(),
                Text = $"{s.ServiceType} - {s.Price:C}"
            }).ToList();

            return View(requestCreateDto);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RequestCreateDto updateDto)    //POSTs and put new function.
        {
            //Requires Code to make it work
            if (!ModelState.IsValid)
                return View();   //if error please load code and see what happened.



            try
            {
                var updated = await _requestService.UpdateRequestAsync(id, updateDto);

                if (!updated)
                {
                    //If error redirect
                    return View("Unauthorized");
                }

                //If that is not the situation, return the proper value
                return RedirectToAction(nameof(Index)); 
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Please validate code.");
                return View(ex.Message); // returns original
            }
        }
    }
}