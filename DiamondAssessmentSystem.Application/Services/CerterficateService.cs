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

        // GET: api/Certificate
        public async Task<IEnumerable<CertificateDto>> GetCertificatesAsync()
        {
            var certificates = await _certificateRepository.GetCertificatesAsync();
            return _mapper.Map<IEnumerable<CertificateDto>>(certificates); 
        }

        public async Task<IEnumerable<CertificateDto>> GetPersonalCertificates(string userId)
        {
            var certificates = await _certificateRepository.GetPersonalCertificates(userId);
            return _mapper.Map<IEnumerable<CertificateDto>>(certificates);
        }

        public async Task<CertificateDto> GetCertificateByIdAsync(int id)
        {
            var certificate = await _certificateRepository.GetCertificateByIdAsync(id);
            if (certificate == null)
            {
                return null; 
            }

            return _mapper.Map<CertificateDto>(certificate);
        }

        // POST: api/Certificate
        public async Task<CertificateDto> CreateCertificateAsync(CertificateCreateDto certificateCreateDto)
        {
            var certificate = _mapper.Map<Certificate>(certificateCreateDto); 

            var createdCertificate = await _certificateRepository.CreateCertificateAsync(certificate);
            return _mapper.Map<CertificateDto>(createdCertificate); 
        }

        public async Task<bool> UpdateCertificateAsync(string userId, CertificateCreateDto certificateCreateDto)
        {
            var existingCertificate = await _certificateRepository.GetCertificateByIdAsync(certificateCreateDto.CertificateId);

            if (existingCertificate == null)
            {
                return false;
            }

            _mapper.Map(certificateCreateDto, existingCertificate);

            if (certificateCreateDto.Status == "Accepted")
            {
                existingCertificate.ApprovedDate = DateTime.UtcNow;
            }

            return await _certificateRepository.UpdateCertificateAsync(userId, existingCertificate); 
        }
    }
}
