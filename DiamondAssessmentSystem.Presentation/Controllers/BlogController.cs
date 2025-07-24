using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICurrentUserService _currentUser;

        public BlogController(IBlogService blogService, ICurrentUserService currentUser)
        {
            _blogService = blogService;
            _currentUser = currentUser;
        }

        public async Task<IActionResult> Index(string? search, string? status)
        {
            var blogs = await _blogService.GetBlogs();

            if (!string.IsNullOrWhiteSpace(search))
                blogs = blogs.Where(b => b.Title.Contains(search, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(status))
                blogs = blogs.Where(b => b.Status == status);

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.StatusOptions = new List<string> { "Draft", "Published", "InActive" };

            return View(blogs);
        }

        public async Task<IActionResult> Details(int id)
        {
            var blog = await _blogService.GetBlogById(id);
            if (blog == null)
                return NotFound();

            return View(blog);
        }

        public IActionResult Create()
        {
            ViewBag.BlogTypeOptions = GetBlogTypeOptions();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BlogDto blogDto)
        {
            ViewBag.BlogTypeOptions = GetBlogTypeOptions(); 

            if (!ModelState.IsValid)
                return View(blogDto);

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return View("Unauthorized");

            try
            {
                var created = await _blogService.CreateBlog(userId, blogDto);
                if (created == null)
                    throw new Exception("Blog creation failed.");

                ViewBag.PopupType = "success";
                ViewBag.PopupMessage = "Blog created successfully.";
                return View(blogDto); 
            }
            catch (Exception ex)
            {
                ViewBag.PopupType = "error";
                ViewBag.PopupMessage = $"Error: {ex.Message}";
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(blogDto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _blogService.GetBlogById(id);
            if (blog == null) return NotFound();

            ViewBag.BlogTypeOptions = GetBlogTypeOptions();
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BlogDto blogDto)
        {
            ViewBag.BlogTypeOptions = GetBlogTypeOptions(); 

            if (id != blogDto.BlogId)
                return NotFound();

            if (!ModelState.IsValid)
                return View(blogDto);

            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return View("Unauthorized");

            try
            {
                blogDto.UpdatedDate = DateTime.UtcNow;
                var updated = await _blogService.UpdateBlog(userId, blogDto);

                if (!updated)
                {
                    ViewBag.PopupType = "error";
                    ViewBag.PopupMessage = "Blog update failed.";
                    return View(blogDto);
                }

                ViewBag.PopupType = "success";
                ViewBag.PopupMessage = "Blog updated successfully.";
                return View(blogDto);
            }
            catch (Exception ex)
            {
                ViewBag.PopupType = "error";
                ViewBag.PopupMessage = $"Error: {ex.Message}";
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(blogDto);
            }
        }

        private List<string> GetBlogTypeOptions() => new()
        {
            "Technology",
            "Tutorial",
            "News",
            "Tips",
            "Case Study",
            "Opinion",
            "Research",
            "Announcement",
            "Event",
            "Guide",
            "Update"
        };

        public async Task<IActionResult> Delete(int id)
        {
            var blog = await _blogService.GetBlogById(id);
            if (blog == null)
                return NotFound();

            return View(blog);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return View("Unauthorized");

            var blogDto = new BlogDto { BlogId = id };

            try
            {
                var deleted = await _blogService.DeleteBlog(userId, blogDto);
                if (!deleted)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(blogDto);
            }
        }

        public async Task<IActionResult> ChangeStatus(int id)
        {
            var blog = await _blogService.GetBlogById(id);
            if (blog == null)
                return NotFound();

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatusConfirmed(int id, string newStatus)
        {
            var userId = _currentUser.UserId;
            if (string.IsNullOrEmpty(userId))
                return View("Unauthorized");

            var blog = await _blogService.GetBlogById(id);
            if (blog == null)
                return NotFound();

            blog.Status = newStatus;
            blog.UpdatedDate = DateTime.UtcNow;

            try
            {
                var result = await _blogService.UpdateBlog(userId, blog);
                if (!result)
                {
                    ViewBag.PopupType = "error";
                    ViewBag.PopupMessage = "Update failed.";
                }
                else
                {
                    ViewBag.PopupType = "success";
                    ViewBag.PopupMessage = "Status updated successfully.";
                }

                return View("ChangeStatus", blog);
            }
            catch (Exception ex)
            {
                ViewBag.PopupType = "error";
                ViewBag.PopupMessage = $"Error: {ex.Message}";
                return View("ChangeStatus", blog);
            }
        }
    }
}
