using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Application.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly ICurrentUserService _currentUser;

        public BlogController(IBlogService blogService, ICurrentUserService currentUser)
        {
            _blogService = blogService;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogDto>>> GetBlogs()
        {
            var blogs = await _blogService.GetBlogs();
            return Ok(blogs);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BlogDto>> GetBlog(int id)
        {
            var blog = await _blogService.GetBlogById(id);

            if (blog == null)
            {
                return NotFound();
            }

            return Ok(blog);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BlogDto>> CreateBlog(BlogDto blogDto)
        {
            var userId = _currentUser.UserId;

            if (userId == null)
                return Unauthorized();

            var createdBlog = await _blogService.CreateBlog(userId, blogDto);
            return CreatedAtAction(nameof(GetBlog), new { id = createdBlog.BlogId }, createdBlog);
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlog(int id, BlogDto blogDto)
        {
            if (id != blogDto.BlogId)
            {
                return BadRequest();
            }

            var userId = _currentUser.UserId;

            if (userId == null)
                return Unauthorized();

            var updated = await _blogService.UpdateBlog(userId, blogDto);

            if (!updated)
            {
                return NotFound();
            }

            return NoContent();
        }

        //[Authorize(Roles = "Consultant")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlog(int id)
        {
            var blog = new BlogDto { 
                BlogId = id
            };

            var userId = _currentUser.UserId;

            if (userId == null)
                return Unauthorized();

            var deleted = await _blogService.DeleteBlog(userId, blog);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
