using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class ResultService : IResultService
    {
        private readonly IResultRepository _resultRepository;
        private readonly ICertificateRepository _certificateRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public ResultService(IResultRepository resultRepository, IMapper mapper, ICertificateRepository certificateRepository, IOrderRepository orderRepository)
        {
            _resultRepository = resultRepository;
            _mapper = mapper;
            _certificateRepository = certificateRepository;
            _orderRepository = orderRepository;
        }

        // Lấy danh sách kết quả
        public async Task<IEnumerable<ResultDto>> GetResultsAsync()
        {
            var results = await _resultRepository.GetResultsAsync();
            return _mapper.Map<IEnumerable<ResultDto>>(results);  
        }

        public async Task<IEnumerable<ResultDto>> GetResultsAsync(int customerId)
        {
            var results = await _resultRepository.GetResultsAsync(customerId);
            return _mapper.Map<IEnumerable<ResultDto>>(results);
        }

        public async Task<IEnumerable<ResultDto>> GetPersonalResults(string userId)
        {
            var results = await _resultRepository.GetPersonalResults(userId);
            return _mapper.Map<IEnumerable<ResultDto>>(results); 
        }

        // Lấy kết quả theo ID
        public async Task<ResultDto> GetResultByIdAsync(int id)
        {
            var result = await _resultRepository.GetResultByIdAsync(id);
            if (result == null)
            {
                return null; 
            }

            return _mapper.Map<ResultDto>(result);  
        }

        // Tạo kết quả mới
        public async Task<bool> CreateResultAsync(int orderId, ResultCreateDto resultCreateDto)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            
            if (order == null)
            {
                return false;
            }

            var result = _mapper.Map<Result>(resultCreateDto);  

            var createdResult = await _resultRepository.CreateResultAsync(result);

            if (createdResult == null)
            {
                return false;
            }
            return true;
        }

        // Cập nhật kết quả
        public async Task<bool> UpdateResultAsync(int id, ResultCreateDto resultCreateDto)
        {
            var existingResult = await _resultRepository.GetResultByIdAsync(id);

            if (existingResult == null)
            {
                return false; 
            }

            _mapper.Map(resultCreateDto, existingResult);  

            if (existingResult.Status == "Completed")
            {
                var cer = new Certificate
                {
                    IssueDate = DateTime.Now,
                    ResultId = id,
                    Status = "Pending"
                };

                await _certificateRepository.CreateCertificateAsync(cer);
            }

            return await _resultRepository.UpdateResultAsync(existingResult); 
        }

        // Xóa kết quả
        public async Task<bool> DeleteResultAsync(int id)
        {
            var existingResult = await _resultRepository.GetResultByIdAsync(id);

            if (existingResult == null)
            {
                return false;
            }

            existingResult.Status = "InActive";
            return await _resultRepository.UpdateResultAsync(existingResult);
        }
    }
}
