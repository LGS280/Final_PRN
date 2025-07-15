using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IMapper _mapper;

        public BlogService(IBlogRepository blogRepository, IMapper mapper)
        {
            _blogRepository = blogRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BlogDto>> GetBlogs()
        {
            var blogs = await _blogRepository.GetBlogsAsync();
            var blogDtos = _mapper.Map<IEnumerable<BlogDto>>(blogs);  
            return blogDtos;
        }

        public async Task<BlogDto> GetBlogById(int id)
        {
            var blog = await _blogRepository.GetBlogByIdAsync(id);
            if (blog == null)
            {
                return null; 
            }

            var blogDto = _mapper.Map<BlogDto>(blog);  
            return blogDto;
        }

        public async Task<BlogDto> CreateBlog(string userId, BlogDto blogDto)
        {
            if (string.IsNullOrWhiteSpace(blogDto.Status))
            {
                blogDto.Status = "Draft";
            }

            var blog = _mapper.Map<Blog>(blogDto);

            var createdBlog = await _blogRepository.CreateBlogAsync(userId, blog);

            var createdBlogDto = _mapper.Map<BlogDto>(createdBlog);

            return createdBlogDto;
        }

        public async Task<bool> UpdateBlog(string userId, BlogDto blogDto)
        {
            if (string.IsNullOrWhiteSpace(blogDto.Status))
            {
                blogDto.Status = "Draft";
            }

            var blog = _mapper.Map<Blog>(blogDto);

            var updated = await _blogRepository.UpdateBlogAsync(userId, blog);

            return updated;
        }

        public async Task<bool> DeleteBlog(string userId, BlogDto blogDto)
        {
            var existingBlog = await _blogRepository.GetBlogByIdAsync(blogDto.BlogId);
            if (existingBlog == null)
            {
                return false;
            }

            existingBlog.Status = "InActive";
            existingBlog.UpdatedDate = DateTime.UtcNow;

            return await _blogRepository.UpdateBlogAsync(userId, existingBlog);
        }
    }
}
