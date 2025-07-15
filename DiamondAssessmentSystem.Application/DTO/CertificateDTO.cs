using System.Collections.Generic;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class CertificateDto
    {
        public int CertificateId { get; set; }

        public int ResultId { get; set; }

        public string? CertificateNumber { get; set; }

        public DateTime? IssueDate { get; set; }

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string? Status { get; set; }
    }

    public class CertificateCreateDto
    {
        public int CertificateId { get; set; }

        public int ResultId { get; set; }

        public string? CertificateNumber { get; set; }

        public DateTime? IssueDate { get; set; }

        public int? ApprovedBy { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public string? Status { get; set; }
    }
}
