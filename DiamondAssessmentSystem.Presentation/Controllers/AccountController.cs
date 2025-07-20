using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;  // For SelectList
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    //[Authorize(Roles = "Admin")]  
    public class AccountController : Controller  
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        // GET: Account
        public async Task<IActionResult> Index() // Renamed from GetAccounts
        {
            var accountDtos = await _accountService.GetAllUsersAsync();
            return View(accountDtos);  // Return a View
        }

        // GET: Account/Details/{id}
        public async Task<IActionResult> Details(string id) // Renamed from GetAccountById
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountDto = await _accountService.GetUserByIdAsync(id);
            if (accountDto == null)
            {
                return NotFound();
            }

            return View(accountDto); // Return a View
        }

        // GET: Account/Create
        public IActionResult CreateEmployee()
        {
            return View(); //To load employee account (view)
        }

        // POST: Account/RegisterEmployee
        [HttpPost]
        [ValidateAntiForgeryToken]  // Recommended for security
        public async Task<IActionResult> CreateEmployee(RegisterEmployeesDto registerDto)
        {
            if (string.IsNullOrEmpty(registerDto.Role))
            {
                ModelState.AddModelError(string.Empty, "Roles cannot be left empty.");
                return View(registerDto);  // Return with validation error, show employee view.
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var account = await _accountService.CreateEmployeeAsync(registerDto, registerDto.Role);
                    return RedirectToAction(nameof(Index));  // Redirect to list
                }
                catch (Exception ex)
                {
                    // Log the exception (Important)
                    ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                    return View(registerDto); // Return with error, show the employee
                }
            }
            return View(registerDto);  // If Modelstate is invalid, load create employee
        }

        // GET: Account/Edit/{id}
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountDto = await _accountService.GetUserByIdAsync(id);
            if (accountDto == null)
            {
                return NotFound();
            }

            return View(accountDto); // Return an edit page
        }

        // POST: Account/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, AccountDto accountDto)
        {
            if (id != accountDto.UserId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var updated = await _accountService.UpdateAccountAsync(id, accountDto);

                    if (!updated)
                    {
                        return NotFound();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the exception
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the account: " + ex.Message);
                    return View(accountDto); //Load edit and return
                }
            }

            // If ModelState is not valid
            return View(accountDto); //Reload page
        }

        // GET: Account/Delete/{id}
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var accountDto = await _accountService.GetUserByIdAsync(id);
            if (accountDto == null)
            {
                return NotFound();
            }

            return View(accountDto); // Return the delete confirmation
        }

        // POST: Account/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var deleted = await _accountService.DeleteAccountAsync(id);

                if (!deleted)
                {
                    return NotFound();
                }

                return RedirectToAction(nameof(Index)); //Redirect
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"The account cannot be deleted.");
                return RedirectToAction(nameof(Index));  //if cannot, send to base line.
            }
        }
    }
}