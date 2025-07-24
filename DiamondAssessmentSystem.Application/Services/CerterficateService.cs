using AutoMapper;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Services
{
    public class CertificateService : ICerterficateService
    {
        private readonly ICertificateRepository _certificateRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IUserRepository _userRepository;
        private readonly DiamondAssessmentDbContext _context;
        private readonly IMapper _mapper;

        public CertificateService(
            ICertificateRepository certificateRepository,
            IMapper mapper,
            ICurrentUserService currentUser,
            DiamondAssessmentDbContext context,
            IUserRepository userRepository)
        {
            _certificateRepository = certificateRepository;
            _mapper = mapper;
            _currentUser = currentUser;
            _context = context;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<CertificateDto>> GetCertificatesAsync()
        {
            var certs = await _certificateRepository.GetCertificatesAsync();
            var certDtos = _mapper.Map<IEnumerable<CertificateDto>>(certs);

            // Bổ sung ApprovedByName từ User table
            foreach (var dto in certDtos)
            {
                if (dto.ApprovedBy.HasValue)
                {
                    var user = await _userRepository.GetUserByEmployeeIdAsync(dto.ApprovedBy.Value);
                    if (user != null)
                        dto.ApprovedByName = $"{user.FirstName} {user.LastName}".Trim();
                }
            }

            return certDtos;
        }

        public async Task<IEnumerable<CertificateDto>> GetPersonalCertificates(string userId)
        {
            var certs = await _certificateRepository.GetPersonalCertificates(userId);
            var certDtos = _mapper.Map<IEnumerable<CertificateDto>>(certs);

            foreach (var dto in certDtos)
            {
                if (dto.ApprovedBy.HasValue)
                {
                    var user = await _userRepository.GetUserByEmployeeIdAsync(dto.ApprovedBy.Value);
                    if (user != null)
                        dto.ApprovedByName = $"{user.FirstName} {user.LastName}".Trim();
                }
            }

            return certDtos;
        }

        public async Task<CertificateDto?> GetCertificateByIdAsync(int id)
        {
            var cert = await _certificateRepository.GetCertificateByIdAsync(id);
            if (cert == null) return null;

            var dto = _mapper.Map<CertificateDto>(cert);
            if (dto.ApprovedBy.HasValue)
            {
                var user = await _userRepository.GetUserByEmployeeIdAsync(dto.ApprovedBy.Value);
                if (user != null)
                    dto.ApprovedByName = $"{user.FirstName} {user.LastName}".Trim();
            }

            return dto;
        }

        public async Task<CertificateDto> CreateCertificateAsync(CertificateCreateDto dto)
        {
            var cert = _mapper.Map<Certificate>(dto);
            cert.Status = "Pending"; // default status
            var created = await _certificateRepository.CreateCertificateAsync(cert);
            return _mapper.Map<CertificateDto>(created);
        }

        public async Task<string> GenerateCertificateNumberAsync()
        {
            var lastNumber = await _context.Certificates
                .OrderByDescending(c => c.CertificateId)
                .Select(c => c.CertificateNumber)
                .FirstOrDefaultAsync();

            int nextNumber = 1;
            if (!string.IsNullOrEmpty(lastNumber) && lastNumber.StartsWith("CERT-"))
            {
                var numericPart = lastNumber.Substring(5);
                if (int.TryParse(numericPart, out int lastNumeric))
                {
                    nextNumber = lastNumeric + 1;
                }
            }

            return $"CERT-{nextNumber.ToString("D6")}";
        }


        public async Task<bool> UpdateCertificateAsync(string userId, CertificateCreateDto dto)
        {
            var existing = await _certificateRepository.GetCertificateByIdAsync(dto.CertificateId);
            if (existing == null || existing.Status == "Approved")
                return false;

            _mapper.Map(dto, existing);

            return await _certificateRepository.UpdateCertificateAsync(userId, existing);
        }

        public async Task<bool> UpdateCertificateAsync(CertificateEditDto dto)
        {
            var cert = await _certificateRepository.GetCertificateByIdAsync(dto.CertificateId);
            if (cert == null) return false;

            var oldStatus = cert.Status;
            cert.Status = dto.Status;

            _mapper.Map(dto, cert);

            // Nếu trạng thái chuyển sang Approved
            if (dto.Status == "Approved")
            {
                if (!cert.ApprovedBy.HasValue)
                {
                    cert.ApprovedBy = _currentUser.AssociatedId;
                    cert.ApprovedDate = DateTime.UtcNow;
                }

                if (string.IsNullOrEmpty(cert.CertificateNumber))
                {
                    cert.CertificateNumber = await GenerateCertificateNumberAsync();
                }
            }
            else if (dto.Status == "Rejected")
            {
                // Nếu từ Approved chuyển sang Rejected thì reset các trường liên quan
                cert.ApprovedBy = null;
                cert.ApprovedDate = null;
                cert.CertificateNumber = null;
            }

            return await _certificateRepository.UpdateAsync(cert);
        }

    }
}
