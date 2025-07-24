using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
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
        private readonly IRequestRepository _requestRepository;
        private readonly IMapper _mapper;
        //private readonly DiamondAssessmentDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public ResultService(
            IResultRepository resultRepository,
            IMapper mapper,
            ICertificateRepository certificateRepository,
            IOrderRepository orderRepository,
            DiamondAssessmentDbContext context,
            ICurrentUserService currentUserService,
            IRequestRepository requestRepository)
        {
            _resultRepository = resultRepository;
            _mapper = mapper;
            _certificateRepository = certificateRepository;
            _orderRepository = orderRepository;
            //_context = context;
            _currentUserService = currentUserService;
            _requestRepository = requestRepository;
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

        public async Task<bool> CreateResultAsync(ResultCreateDto dto)
        {
            var request = await _requestRepository.GetRequestByIdAsync(dto.RequestId);

            if (request == null)
                throw new Exception("Request is invalid!");

            var result = _mapper.Map<Result>(dto);
            result.ModifiedDate = DateTime.Now;

            try
            {
                var created = await _resultRepository.CreateResultAsync(result);
                var updated = await _requestRepository.UpdateRequestAsync(request);
                if (!updated) return false;

                if (created != null && created.Status == "Completed")
                {
                    var certificate = new Certificate
                    {
                        ResultId = created.ResultId,
                        IssueDate = DateTime.UtcNow,
                        Status = "Pending"
                    };

                    await _certificateRepository.CreateCertificateAsync(certificate);
                }

                return created != null;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi khi tạo result: " + ex.Message);
                Console.WriteLine("Chi tiết: " + ex.InnerException?.Message);
                return false;
            }

        }

        public async Task<bool> UpdateResultAsync(int id, ResultUpdateDto dto)
        {
            var existingResult = await _resultRepository.GetResultByIdAsync(id);
            if (existingResult == null)
                return false;

            var cert = await _certificateRepository.GetByResultIdAsync(id);
            if (cert?.Status == "Issued")
                return false;

            // Cập nhật thông tin
            _mapper.Map(dto, existingResult);
            existingResult.ModifiedDate = DateTime.Now;

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
