using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Presentation.Controllers
{
    //[Authorize(Roles = "Staff,Customer")]
    public class BlogController : Controller  // Inherit from Controller
    {
        private readonly IBlogService _blogService;
        private readonly ICurrentUserService _currentUser;

        public BlogController(IBlogService blogService, ICurrentUserService currentUser)
        {
            _blogService = blogService;
            _currentUser = currentUser;
        }

        // GET: Blog
        public async Task<IActionResult> Index() // Changed from GetBlogs
        {
            var blogs = await _blogService.GetBlogs();
            return View(blogs);  // return View with list of all Blogs
        }

        // GET: Blog/Details/{id}
        public async Task<IActionResult> Details(int id) // Changed from GetBlog
        {
            var blog = await _blogService.GetBlogById(id);

            if (blog == null)
            {
                return NotFound();
            }

            return View(blog); // Return the view if Blog is found
        }

        // GET: Blog/Create
        public IActionResult Create() //Loads the data to create a profile
        {
            return View();  // Returns to View that allows for creating of blog
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //For Security
        public async Task<IActionResult> Create(BlogDto blogDto)
        {
            if (ModelState.IsValid)
            {
                try
                {

                    var userId = _currentUser.UserId;

                    if (userId == null)
                    {
                        //Add Model for users whom cannot log in, so the app isn't in null state.
                        throw new Exception("User is not authenticated.");
                        return View(blogDto);  //Returns error, not just an empty page

                    }

                    var createdBlog = await _blogService.CreateBlog(userId, blogDto);
                    return RedirectToAction(nameof(Index)); // Send back to Index if good
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(blogDto); // return view of exception
                }

            }
            return View(blogDto); // If Modelstate is invalid, reload
        }
        // GET: Blog/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var blog = await _blogService.GetBlogById(id);
            if (blog == null) return NotFound();

            return View(blog); //Load code
        }
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, BlogDto blogDto)
        {
            if (id != blogDto.BlogId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    var userId = _currentUser.UserId;
                    if (userId == null)
                        return View(Unauthorized());

                    //Attempt to Update the code for the User

                    //Use try/catch or similar methods to validate
                    blogDto.UpdatedDate = DateTime.UtcNow; // Set the updated date to now
                    var updated = await _blogService.UpdateBlog(userId, blogDto);

                    if (!updated)
                    {
                        return NotFound();
                    }

                    return RedirectToAction(nameof(Index)); //Redirect
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ERROR] Exception: {ex.Message}");
                    ModelState.AddModelError(string.Empty, "Cannot Update this Value, try again");
                    return View(blogDto);
                }
            }

            return View();   //This should be default, to have accurate code it should stay.
        }

        // GET: Blog/Delete/{id}
        public async Task<IActionResult> Delete(int id)  //Gets the Method and Loads to Confirm
        {
            var blog = await _blogService.GetBlogById(id);
            if (blog == null) return NotFound();

            return View(blog); //Check and prepare deletion 
        }

        // POST: Blog/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) //Posts and run and Deletes.
        {
            var blog = new BlogDto { BlogId = id };

            var userId = _currentUser.UserId;

            if (userId == null)
                return View(Unauthorized());

            //Validate to the best of ability
            var deleted = await _blogService.DeleteBlog(userId, blog);

            if (!deleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index)); //Deletes and redirects as is told in the guide.
        }
    }
}