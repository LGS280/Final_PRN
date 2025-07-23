using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Application.Services;
using DiamondAssessmentSystem.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBlogService _blogService;
        private readonly IServicePriceService _servicePriceService;

        public HomeController(ILogger<HomeController> logger, IBlogService blogService, IServicePriceService servicePriceService)
        {
            _logger = logger;
            _blogService = blogService;
            _servicePriceService = servicePriceService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? search, string? blogType)
        {
            var blogs = await _blogService.GetBlogs();

            blogs = blogs.Where(b => b.Status == "Published");

            if (!string.IsNullOrWhiteSpace(search))
                blogs = blogs.Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(blogType))
                blogs = blogs.Where(b => b.BlogType == blogType);

            ViewBag.Search = search;
            ViewBag.BlogType = blogType;
            ViewBag.BlogTypeOptions = blogs.Select(b => b.BlogType).Where(t => !string.IsNullOrEmpty(t)).Distinct().ToList();

            return View(blogs);
        }

        public async Task<IActionResult> Services()
        {
            var services = await _servicePriceService.GetByStatusAsync("Active");
            return View(services);
        }

        public IActionResult GetStarted()
        {
            if (!User.Identity.IsAuthenticated)
                return RedirectToAction("Login", "Auth");

            return RedirectToAction("My", "Request");
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
