namespace DiamondAssessmentSystem.Application.DTO
{
    public class VnPaymentRequestDto
    {
        public int RequestId { get; set; }
        public int ServiceId { get; set; }
        public double Amount { get; set; } 
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
