using DiamondAssessmentSystem.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IRequestService
    {
        Task<IEnumerable<RequestDto>> GetRequestsAsync();
        Task<RequestDto?> GetRequestByIdAsync(int id);
        Task<IEnumerable<RequestDto>> GetRequestsByCustomerIdAsync(string userId);
        Task<RequestDto?> CreateRequestForCustomerAsync(string userId, RequestCreateDto dto, string status);
        Task<bool> CancelRequestAsync(string userId, int requestId);
        Task<bool> UpdateRequestAsync(int id, RequestCreateDto updateDto);
    }
}
