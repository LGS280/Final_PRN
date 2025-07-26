using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;  // For SelectList
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class AuthController : Controller 
    {
        private readonly IAuthService _authService;
        private readonly UserManager<Infrastructure.Models.User> _userManager;

        public AuthController(IAuthService authService, UserManager<Infrastructure.Models.User> userManager)
        {
            _authService = authService;
            _userManager = userManager;
        }

        // GET: Auth/Login
        [HttpGet]
        public IActionResult Login() 
        {
            return View(); 
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto loginDto) 
        {
            if (!ModelState.IsValid)
                return View(loginDto);

            try
            {
                var loginResponse = await _authService.LoginAsync(loginDto);

                HttpContext.Session.SetString("access_token", loginResponse.Token);

                return RedirectToAction("Index", "Home");

            }
            catch (UnauthorizedAccessException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(loginDto);

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");
                return View(loginDto);
            }
        }

        // GET: Auth/Register
        [HttpGet]
        public IActionResult RegisterCustomer() 
        {
            return View();
        }

        // POST: Auth/RegisterCustomer
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCustomer(RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
                return View(registerDto);

            try
            {
                var newUser = await _authService.RegisterCustomerAsync(registerDto);
                await _authService.SendConfirmationEmailAsync(newUser);

                return RedirectToAction("ConfirmEmailNotice", "Auth", new { email = newUser.Email });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(registerDto);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return View("ConfirmEmailFailed");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("ConfirmEmailFailed");
            }

            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (result.Succeeded)
            {
                return View("ConfirmEmailSuccess", user.Email); // Truyền email nếu muốn hiển thị
            }
            else
            {
                return View("ConfirmEmailFailed");
            }
        }

        [HttpGet]
        public IActionResult ConfirmEmailNotice(string email)
        {
            ViewBag.Email = email;
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");

        }

    }
}