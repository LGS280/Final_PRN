using System.ComponentModel.DataAnnotations;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public int ServiceId { get; set; }
        public string ServiceType { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = string.Empty;
        public int RequestId { get; set; }
    }

    public class OrderCreateDto
    {
        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public int ServiceId { get; set; }

        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }
    }

    public class OrderPaymentDto
    {
        [Required]
        public int RequestId { get; set; }

        [Required, MaxLength(50)]
        public string PaymentType { get; set; } = string.Empty;

        public VnPaymentResponseFromFe? PaymentRequest { get; set; }
    }

    public class OrderCreateCombineDto
    {
        [Required]
        public OrderCreateDto OrderData { get; set; } = null!;

        [Required]
        public OrderPaymentDto PaymentInfo { get; set; } = null!;
    }

    public class UpdatePaymentDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required, MaxLength(50)]
        public string Status { get; set; } = string.Empty;
    }
}
