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

        public ResultService(
            IResultRepository resultRepository,
            IMapper mapper,
            ICertificateRepository certificateRepository,
            IOrderRepository orderRepository)
        {
            _resultRepository = resultRepository;
            _mapper = mapper;
            _certificateRepository = certificateRepository;
            _orderRepository = orderRepository;
        }

        public async Task<IEnumerable<ResultDto>> GetResultsAsync() =>
            _mapper.Map<IEnumerable<ResultDto>>(await _resultRepository.GetResultsAsync());

        public async Task<IEnumerable<ResultDto>> GetResultsAsync(int customerId) =>
            _mapper.Map<IEnumerable<ResultDto>>(await _resultRepository.GetResultsAsync(customerId));

        public async Task<IEnumerable<ResultDto>> GetPersonalResults(string userId) =>
            _mapper.Map<IEnumerable<ResultDto>>(await _resultRepository.GetPersonalResults(userId));

        public async Task<ResultDto?> GetResultByIdAsync(int id)
        {
            var result = await _resultRepository.GetResultByIdAsync(id);
            return result == null ? null : _mapper.Map<ResultDto>(result);
        }

        public async Task<bool> CreateResultAsync(int orderId, ResultCreateDto dto)
        {
            var order = await _orderRepository.GetOrderByIdAsync(orderId);
            if (order == null || order.Status != "Completed")
                return false;

            if (order.Service == null || dto.RequestId != order.Service.ServiceId)
                return false;

            var result = _mapper.Map<Result>(dto);
            return await _resultRepository.CreateResultAsync(result) != null;
        }

        public async Task<bool> UpdateResultAsync(int id, ResultCreateDto dto)
        {
            var existingResult = await _resultRepository.GetResultByIdAsync(id);
            if (existingResult == null)
                return false;

            var cert = await _certificateRepository.GetByResultIdAsync(id);
            if (cert?.Status == "Issued")
                return false;

            _mapper.Map(dto, existingResult);

            // Tạo Certificate nếu chưa có và trạng thái là Completed
            if (existingResult.Status == "Completed" && cert == null)
            {
                var newCert = new Certificate
                {
                    ResultId = existingResult.ResultId,
                    IssueDate = DateTime.UtcNow,
                    Status = "Pending"
                };
                await _certificateRepository.CreateCertificateAsync(newCert);
            }

            return await _resultRepository.UpdateResultAsync(existingResult);
        }

        public async Task<bool> DeleteResultAsync(int id)
        {
            var result = await _resultRepository.GetResultByIdAsync(id);
            if (result == null)
                return false;

            var cert = await _certificateRepository.GetByResultIdAsync(id);
            if (cert?.Status == "Issued")
                return false;

            result.Status = "InActive";
            return await _resultRepository.UpdateResultAsync(result);
        }
    }
}
