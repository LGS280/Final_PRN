using DiamondAssessmentSystem.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogDto>> GetBlogs();

        Task<BlogDto> GetBlogById(int id);

        Task<BlogDto> CreateBlog(string userId, BlogDto blogDto);

        Task<bool> UpdateBlog(string userId, BlogDto blogDto);

        Task<bool> DeleteBlog(string userId, BlogDto blogDto);
    }
}
