using DiamondAssessmentSystem.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IResultService
    {
        Task<IEnumerable<ResultDto>> GetResultsAsync();
        Task<IEnumerable<ResultDto>> GetResultsAsync(int customerId);
        Task<IEnumerable<ResultDto>> GetPersonalResults(string userId);
        Task<ResultDto> GetResultByIdAsync(int id);
        Task<bool> CreateResultAsync(ResultCreateDto resultCreateDto);
        Task<bool> UpdateResultAsync(int id, ResultCreateDto resultCreateDto);
        Task<bool> DeleteResultAsync(int id);
    }
}
