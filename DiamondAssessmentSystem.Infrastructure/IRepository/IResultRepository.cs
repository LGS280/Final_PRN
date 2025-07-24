using DiamondAssessmentSystem.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface IResultRepository
    {
        Task<IEnumerable<Result>> GetResultsAsync();
        Task<IEnumerable<Result>> GetResultsAsync(int customerId);
        Task<IEnumerable<Result>> GetPersonalResults(string userId);
        Task<Result?> GetResultByIdAsync(int id);
        Task<Result> CreateResultAsync(Result result);
        Task<bool> UpdateResultAsync(Result result);
    }
}
