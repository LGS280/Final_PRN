using DiamondAssessmentSystem.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.IRepository
{
    public interface ICertificateRepository
    {
        Task<IEnumerable<Certificate>> GetCertificatesAsync();
        Task<IEnumerable<Certificate>> GetPersonalCertificates(string userId);
        Task<Certificate?> GetByResultIdAsync(int resultId);
        Task<Certificate?> GetCertificateByIdAsync(int id);
        Task<Certificate?> GetPersonalCertificateById(string userId);
        Task<Certificate> CreateCertificateAsync(Certificate certificate);
        Task<bool> UpdateCertificateAsync(string userId, Certificate certificate);
    }
}
