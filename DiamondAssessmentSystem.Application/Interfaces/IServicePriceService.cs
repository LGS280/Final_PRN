﻿using DiamondAssessmentSystem.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IServicePriceService
    {
        Task<IEnumerable<ServicePriceDto>> GetAllAsync();
        Task<IEnumerable<ServicePriceDto>> GetByStatusAsync(string status);
        Task<ServicePriceDto?> GetByIdAsync(int id);
        Task<ServicePriceDto> CreateAsync(ServicePriceCreateDto dto, string userId);
        Task<bool> UpdateAsync(int id, ServicePriceCreateDto dto, string userId);
        Task<bool> SoftDeleteAsync(int id);
    }
}
