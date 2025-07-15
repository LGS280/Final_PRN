using DiamondAssessmentSystem.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerDto> GetCustomerByIdAsync(string userId);
        Task<bool> UpdateCustomerAsync(string userId, CustomerCreateDto customerCreateDto);
    }
}
