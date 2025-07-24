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
    [Authorize(Roles = "Staff,Customer,Assessor, Manager")]
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
        public async Task<IActionResult> Index() 
        {
            var requests = await _requestService.GetRequestsAsync();
            return View(requests);  
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
        public async Task<IActionResult> My() 
        {
            if (!User.Identity.IsAuthenticated) 
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                return View("UnAuthorized");
            }

            var requests = await _requestService.GetRequestsByCustomerIdAsync(userId);

            return View(requests);  
        }

        // GET: Request/Create
        public async Task<IActionResult> Create()
        {
            var activeServices = await _servicePriceService.GetByStatusAsync("Active");

            ViewBag.Services = activeServices.Select(s => new SelectListItem
            {
                Value = s.ServiceId.ToString(),
                Text = $"{s.ServiceType} - {s.Price:C}"
            }).ToList();

            ViewBag.ServiceDetails = activeServices.Select(s => new
            {
                serviceId = s.ServiceId,
                serviceType = s.ServiceType,
                price = s.Price,
                priceFormatted = s.Price.ToString("C"),
                duration = s.Duration,
                description = s.Description,
                category = s.ServiceType.Split('-')[0].Trim(), 
                status = s.Status
            }).ToList();

            ViewBag.Categories = activeServices
                .Select(s => s.ServiceType.Split('-')[0].Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ViewBag.Statuses = activeServices
                .Select(s => s.Status)
                .Distinct()
                .OrderBy(x => x)
                .ToList();

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
                await LoadServicesAsync();
                return View(createDto);
            }

            try
            {
                var userId = _currentUser.UserId;
                if (string.IsNullOrEmpty(userId))
                    throw new Exception("Invalid user.");

                string status = action == "Submit" ? "Pending" : "Draft";
                var result = await _requestService.CreateRequestForCustomerAsync(userId, createDto, status);

                if (result == null)
                    throw new Exception("Request creation failed.");

                ViewBag.PopupType = "success";
                ViewBag.PopupMessage = "Request submitted successfully.";
            }
            catch (Exception ex)
            {
                ViewBag.PopupType = "error";
                ViewBag.PopupMessage = $"Error: {ex.Message}";
            }

            await LoadServicesAsync();
            return View(createDto);
        }

        [HttpGet]
        [AllowAnonymous] 
        public async Task<IActionResult> Create(int serviceId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Create", "Request", new { serviceId }) });
            }

            var dto = new RequestCreateDto
            {
                ServiceId = serviceId
            };

            await LoadServicesAsync();
            return View(dto);
        }

        private async Task LoadServicesAsync()
        {
            var activeServices = await _servicePriceService.GetByStatusAsync("Active");

            ViewBag.Services = activeServices.Select(s => new SelectListItem
            {
                Value = s.ServiceId.ToString(),
                Text = $"{s.ServiceType} - {s.Price:C}"
            }).ToList();

            ViewBag.Categories = activeServices
                .Select(s => s.ServiceType.Split('-')[0].Trim())
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            ViewBag.Statuses = activeServices
                .Select(s => s.Status)
                .Distinct()
                .ToList();

            ViewBag.ServiceDetails = activeServices.Select(s => new
            {
                serviceId = s.ServiceId,
                serviceType = s.ServiceType,
                price = s.Price,
                priceFormatted = s.Price.ToString("C"),
                duration = s.Duration,
                description = s.Description,
                category = s.ServiceType.Split('-')[0].Trim(),
                status = s.Status
            }).ToList();
        }

        // GET: Request/Cancel/{id}
        [HttpPost, ActionName("Cancel")]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Auth");
            }

            var userId = _currentUser.UserId;

            if (userId == null)
            {
                return RedirectToAction("Error");
            }

            try
            {
                var success = await _requestService.CancelRequestAsync(userId, id);
                return RedirectToAction("Index");   
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Request could not be validate \n  Error is {ex}");
                return View();  
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            if (request.Status == "Pending")
            {
                TempData["ErrorMessage"] = "Cannot edit a request that is already pending.";
                return RedirectToAction("Details", new { id });
            }

            var requestCreateDto = new RequestCreateDto
            {
                ServiceId = request.ServiceId,
                RequestType = request.RequestType,
            };

            ViewBag.Id = request.RequestId;

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
        public async Task<IActionResult> Edit(int id, RequestCreateDto updateDto)    
        {
            if (!ModelState.IsValid)
                return View(updateDto);   

            try
            {
                var updated = await _requestService.UpdateRequestAsync(id, updateDto);

                if (!updated)
                {
                    return View("Unauthorized");
                }

                return RedirectToAction(nameof(My)); 
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Please validate code.");
                return View(ex.Message); 
            }
        }

        [Authorize(Roles = "Assessor,Manager")]
        [HttpGet]
        public async Task<IActionResult> EditAssessor(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            if (request.Status == "Completed" || request.Status == "Canceled")
            {
                TempData["Error"] = "Cannot edit a completed or canceled request.";
                return RedirectToAction("IndexAssessor");
            }

            var dto = new RequestCreateDto
            {
                RequestType = request.RequestType,
                ServiceId = request.ServiceId
            };

            await LoadServicesAsync();
            ViewBag.Id = id;

            return View("EditAssessor", dto);
        }

        [Authorize(Roles = "Assessor,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAssessor(int id, RequestCreateDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                await LoadServicesAsync();
                ViewBag.Id = id;
                return View("EditAssessor", updateDto);
            }

            try
            {
                var success = await _requestService.UpdateRequestAsync(id, updateDto);
                if (!success)
                {
                    TempData["Error"] = "Update failed.";
                    return RedirectToAction("IndexAssessor");
                }

                TempData["Success"] = "Update successful.";
                return RedirectToAction("IndexAssessor");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Unexpected error: {ex.Message}");
                await LoadServicesAsync();
                ViewBag.Id = id;
                return View("EditAssessor", updateDto);
            }
        }


    }
}