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
        public string? ApprovedByName { get; set; }

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

    public class CertificateEditDto
    {
        public int CertificateId { get; set; } //id tự tạo

        public int ResultId { get; set; } //result tự lấy

        public string? CertificateNumber { get; set; } //tự generate với mã là CT00XXX

        public DateTime? IssueDate { get; set; }

        public int? ApprovedBy { get; set; }

        public string? ApprovedByName { get; set; }

        public DateTime? ApprovedDate { get; set; } //tự tạo

        public string? Status { get; set; }
    }
}

