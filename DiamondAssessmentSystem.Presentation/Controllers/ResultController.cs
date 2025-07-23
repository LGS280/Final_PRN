using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;  // For SelectList
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    //[Authorize(Roles = "Consultant")]
    public class ResultController : Controller
    {
        private readonly IResultService _resultService;
        private readonly IRequestService _requestService;
        private readonly ICurrentUserService _currentUser;

        public ResultController(IResultService resultService, ICurrentUserService currentUser, IRequestService requestService)
        {
            _resultService = resultService;
            _currentUser = currentUser;
            _requestService = requestService;
        }

        public async Task<IActionResult> Index()
        {
            var results = await _resultService.GetResultsAsync();
            return View(results);
        }

        public async Task<IActionResult> MyResult()
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Auth");

            var results = await _resultService.GetPersonalResults(userId);
            return View(results);
        }

        public async Task<IActionResult> Details(int id)
        {
            var result = await _resultService.GetResultByIdAsync(id);
            if (result == null) return NotFound();
            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int id)
        {
            var request = await _requestService.GetRequestByIdAsync(id);
            if (request == null)
            {
                TempData["Error"] = "Request is invalid!";
                return RedirectToAction("Index", "Request");
            }

            var model = new ResultCreateDto
            {
                RequestId = id,
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ResultCreateDto dto)
        {
            try
            {
                // Validate RequestId trước (nếu cần)
                var request = await _requestService.GetRequestByIdAsync(dto.RequestId);
                if (request == null)
                {
                    TempData["Error"] = "Request is invalid!";
                    return RedirectToAction("Index", "Request");
                }

                // Gán AssessmentStaff = EmployeeId hiện tại
                if (_currentUser.AssociatedId.HasValue)
                {
                    dto.AssessmentStaff = _currentUser.AssociatedId.Value;
                }
                else
                {
                    TempData["Error"] = "You are not authorized to create results.";
                    return RedirectToAction("Index", "Request");
                }

                await _resultService.CreateResultAsync(dto);

                TempData["Success"] = "Result created successfully!";
                return RedirectToAction("Index", "Request");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Index", "Request");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _resultService.GetResultByIdAsync(id);
            if (result == null) return NotFound();

            var dto = new ResultCreateDto
            {
                RequestId = result.RequestId,
                DiamondOrigin = result.DiamondOrigin,
                Shape = result.Shape,
                Measurements = result.Measurements,
                CaratWeight = result.CaratWeight,
                Color = result.Color,
                Clarity = result.Clarity,
                Cut = result.Cut,
                Proportions = result.Proportions,
                Polish = result.Polish,
                Symmetry = result.Symmetry,
                Fluorescence = result.Fluorescence,
                Status = result.Status
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ResultCreateDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            var updated = await _resultService.UpdateResultAsync(id, dto);
            if (!updated) return View("Unauthorized");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _resultService.GetResultByIdAsync(id);
            return result == null ? NotFound() : View(result);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _resultService.DeleteResultAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}