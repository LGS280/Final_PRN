using DiamondAssessmentSystem.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface IRequestRepository
    {
        // Lấy danh sách tất cả các yêu cầu
        Task<IEnumerable<Request>> GetRequestsAsync();

        // Lấy thông tin chi tiết một yêu cầu theo ID
        Task<Request?> GetRequestByIdAsync(int id);

        Task<IEnumerable<Request>> GetRequestsByCustomerIdAsync(string userId);

        Task<bool> CreateDraftRequest(string userId, Request request);

        Task<bool> CancelRequestAsync(string userId, int requestId);

        // Tạo một yêu cầu mới
        Task<Request> CreateRequestAsync(Request request);

        // Cập nhật một yêu cầu
        Task<bool> UpdateRequestAsync(Request request);

    }
}
