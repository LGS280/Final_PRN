using DiamondAssessmentSystem.Infrastructure.IRepository;
using DiamondAssessmentSystem.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Infrastructure.Repository
{
    public class CertificateRepository : ICertificateRepository
    {
        private readonly DiamondAssessmentDbContext _context;

        public CertificateRepository(DiamondAssessmentDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Certificate>> GetCertificatesAsync()
        {
            return await _context.Certificates
                                 .Include(c => c.Result)  
                                 .ToListAsync();
        }

        public async Task<Certificate?> GetByResultIdAsync(int resultId)
        {
            return await _context.Certificates
                .FirstOrDefaultAsync(c => c.ResultId == resultId);
        }

        public async Task<IEnumerable<Certificate>> GetPersonalCertificates(string userId)
        {
            var customerId = await GetCustomerId(userId);

            if (customerId == -1)
            {
                return Enumerable.Empty<Certificate>();
            }

            return await _context.Certificates.Include(c => c.Result).ThenInclude(r => r.Request)
                .Where(c => c.Result.Request.CustomerId == customerId).ToListAsync();
        }

        public async Task<Certificate?> GetCertificateByIdAsync(int id)
        {
            return await _context.Certificates
                                 .Include(c => c.Result)
                                 .FirstOrDefaultAsync(c => c.CertificateId == id);
        }

        public async Task<Certificate?> GetPersonalCertificateById(string userId)
        {
            var customerId = await GetCustomerId(userId);

            if (customerId == -1)
            {
                return null;
            }

            return await _context.Certificates
                                 .Include(c => c.Result)
                                 .FirstOrDefaultAsync(c => c.CertificateId == customerId);
        }

        public async Task<Certificate> CreateCertificateAsync(Certificate certificate)
        {
            _context.Certificates.Add(certificate);
            await _context.SaveChangesAsync();
            return certificate;
        }

        public async Task<bool> UpdateCertificateAsync(string userId, Certificate certificate)
        {
            var employeeId = await GetEmployeeId(userId);

            _context.Entry(certificate).State = EntityState.Modified;
            certificate.ApprovedBy = employeeId;
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CertificateExistsAsync(certificate.CertificateId))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> UpdateAsync(Certificate certificate)
        {
            _context.Certificates.Update(certificate);
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<bool> CertificateExistsAsync(int id)
        {
            return await _context.Certificates.AnyAsync(e => e.CertificateId == id);
        }

        private async Task<int> GetCustomerId(string userId)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.UserId == userId);

            if (customer == null)
            {
                return -1;
            }

            return customer.CustomerId;
        }

        private async Task<int> GetEmployeeId(string userId)
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(x => x.UserId == userId);

            if (employee == null)
            {
                return -1;
            }

            return employee.EmployeeId;
        }

        public async Task<Certificate?> GetLatestCertificateAsync()
        {
            return await _context.Certificates
                .OrderByDescending(c => c.CertificateId)
                .FirstOrDefaultAsync();
        }
    }
}
