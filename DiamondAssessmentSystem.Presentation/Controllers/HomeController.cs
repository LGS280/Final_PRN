using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogService _blogService;

        public HomeController(ILogger<HomeController> logger, IBlogService blogService)
        {
            _logger = logger;
            _blogService = blogService;

        }

        public async Task<IActionResult> Index()
        {
            var blogs = await _blogService.GetBlogs();
            return View(blogs);
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Account");
            }
            else if (User.IsInRole("Employee"))
            {
                return RedirectToAction("Index", "Blog");
            }
            else if (User.IsInRole("Customer"))
            {
                return RedirectToAction("Me", "Customer");
            } else if (User.IsInRole("Consultant"))
            {
                return RedirectToAction("Index", "Result");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
