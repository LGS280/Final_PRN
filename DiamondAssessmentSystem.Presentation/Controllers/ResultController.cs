using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;  // For SelectList

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    //[Authorize(Roles = "Consultant")]
    public class ResultController : Controller
    {
        private readonly IResultService _resultService;
        private readonly ICurrentUserService _currentUser;

        public ResultController(IResultService resultService, ICurrentUserService currentUser)
        {
            _resultService = resultService;
            _currentUser = currentUser;
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
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int orderId, ResultCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View(dto);
            }

            try
            {
                var success = await _resultService.CreateResultAsync(dto);
                if (!success)
                {
                    TempData["ErrorMessage"] = "You are not authorized to perform this action.";
                    return View(dto);
                }

                TempData["SuccessMessage"] = "Result created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return View(dto);
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