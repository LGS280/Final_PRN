using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class ServicePriceService : IServicePriceService
    {
        private readonly IServicePriceRepository _repository;
        private readonly IMapper _mapper;

        public ServicePriceService(IServicePriceRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ServicePriceDto>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<ServicePriceDto>>(entities);
        }

        public async Task<IEnumerable<ServicePriceDto>> GetByStatusAsync(string status)
        {
            var entities = await _repository.GetByStatusAsync(status);
            return _mapper.Map<IEnumerable<ServicePriceDto>>(entities);
        }

        public async Task<ServicePriceDto?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<ServicePriceDto>(entity);
        }

        public async Task<ServicePriceDto> CreateAsync(ServicePriceCreateDto dto)
        {
            var entity = _mapper.Map<ServicePrice>(dto);
            var created = await _repository.AddAsync(entity);
            return _mapper.Map<ServicePriceDto>(created);
        }

        public async Task<bool> UpdateAsync(int id, ServicePriceCreateDto dto)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return false;

            // Update fields
            existing.ServiceType = dto.ServiceType;
            existing.Price = dto.Price;
            existing.Duration = dto.Duration;
            existing.EmployeeId = dto.EmployeeId;
            existing.Status = dto.Status;

            return await _repository.UpdateAsync(existing);
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var existing = await _repository.GetByIdAsync(id);
            if (existing == null)
                return false;

            existing.Status = "Inactive";
            return await _repository.UpdateAsync(existing);
        }
    }
}
