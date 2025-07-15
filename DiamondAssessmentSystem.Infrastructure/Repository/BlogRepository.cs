using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class BlogRepository : IBlogRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public BlogRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Blog>> GetBlogsAsync()
        {
            return await _context.Blogs.ToListAsync();
        }

        public async Task<Blog?> GetBlogByIdAsync(int id)
        {
            return await _context.Blogs.FindAsync(id);
        }

        public async Task<Blog> CreateBlogAsync(string userId, Blog blog)
        {
            var employeeId = await GetEmployeeId(userId);
            blog.EmployeeId = employeeId;

            _context.Blogs.Add(blog);
            await _context.SaveChangesAsync();
            return blog;
        }

        public async Task<bool> UpdateBlogAsync(string userId, Blog blog)
        {
            var employeeId = await GetEmployeeId(userId);

            _context.Entry(blog).State = EntityState.Modified;
            blog.EmployeeId = employeeId;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BlogExistsAsync(blog.BlogId))
                {
                    return false;
                }
                throw;
            }
        }

        private async Task<bool> BlogExistsAsync(int id)
        {
            return await _context.Blogs.AnyAsync(e => e.BlogId == id);
        }

        private async Task<int> GetEmployeeId(string userId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.UserId == userId);

            if (employee == null)
            {
                return -1;
            }

            return employee.EmployeeId;
        }
    }
}
