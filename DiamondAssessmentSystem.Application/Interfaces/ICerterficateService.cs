using DiamondAssessmentSystem.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface ICerterficateService
    {
        Task<IEnumerable<CertificateDto>> GetCertificatesAsync();
        Task<IEnumerable<CertificateDto>> GetPersonalCertificates(string userId);
        Task<CertificateDto> GetCertificateByIdAsync(int id);
        Task<CertificateDto> CreateCertificateAsync(CertificateCreateDto certificateCreateDto);
        Task<bool> UpdateCertificateAsync(string userId, CertificateCreateDto certificateCreateDto);
        Task<bool> UpdateCertificateAsync(CertificateEditDto dto);
    }
}
