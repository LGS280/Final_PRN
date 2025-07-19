using DiamondAssessmentSystem.Presentation.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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
