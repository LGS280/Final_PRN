using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class CertificateService : ICerterficateService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly IMapper _mapper;

        public CertificateService(ICertificateRepository certificateRepository, IMapper mapper)
        {
            _certificateRepository = certificateRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CertificateDto>> GetCertificatesAsync() =>
            _mapper.Map<IEnumerable<CertificateDto>>(await _certificateRepository.GetCertificatesAsync());

        public async Task<IEnumerable<CertificateDto>> GetPersonalCertificates(string userId) =>
            _mapper.Map<IEnumerable<CertificateDto>>(await _certificateRepository.GetPersonalCertificates(userId));

        public async Task<CertificateDto?> GetCertificateByIdAsync(int id)
        {
            var cert = await _certificateRepository.GetCertificateByIdAsync(id);
            return cert == null ? null : _mapper.Map<CertificateDto>(cert);
        }

        public async Task<CertificateDto> CreateCertificateAsync(CertificateCreateDto dto)
        {
            var cert = _mapper.Map<Certificate>(dto);
            var created = await _certificateRepository.CreateCertificateAsync(cert);
            return _mapper.Map<CertificateDto>(created);
        }

        public async Task<bool> UpdateCertificateAsync(string userId, CertificateCreateDto dto)
        {
            var existing = await _certificateRepository.GetCertificateByIdAsync(dto.CertificateId);
            if (existing == null || existing.Status == "Issued")
                return false;

            _mapper.Map(dto, existing);

            if (dto.Status == "Issued")
            {
                if (dto.ApprovedBy == null)
                    return false;

                existing.ApprovedDate = DateTime.UtcNow;
            }

            return await _certificateRepository.UpdateCertificateAsync(userId, existing);
        }
    }
}
