using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.DTO
{
    public class VnPaymentResponseDto
    {
        public bool Success { get; set; }
        public string? PaymentMethod { get; set; }
        public string? OrderDescription { get; set; }
        public int? OrderId { get; set; }
        public string? TransactionId { get; set; }
        public string? Token { get; set; }
        public string? VnPayResponseCode { get; set; }
        public string? Message { get; set; }
    }

    public class VnPaymentResponseFromFe
    {
        [FromQuery(Name = "vnp_Amount")]
        public string? Amount { get; set; }

        [FromQuery(Name = "vnp_BankCode")]
        public string? BankCode { get; set; }

        [FromQuery(Name = "vnp_BankTranNo")]
        public string? BankTranNo { get; set; }

        [FromQuery(Name = "vnp_CardType")]
        public string? CardType { get; set; }

        [FromQuery(Name = "vnp_OrderInfo")]
        public string? OrderInfo { get; set; }

        [FromQuery(Name = "vnp_PayDate")]
        public string? PayDate { get; set; }

        [FromQuery(Name = "vnp_ResponseCode")]
        public string? VnPayResponseCode { get; set; }

        [FromQuery(Name = "vnp_TmnCode")]
        public string? TmnCode { get; set; }

        [FromQuery(Name = "vnp_TransactionNo")]
        public string? TransactionNo { get; set; }

        [FromQuery(Name = "vnp_TransactionStatus")]
        public string? TransactionStatus { get; set; }

        [FromQuery(Name = "vnp_TxnRef")]
        public string? TxnRef { get; set; }

        [FromQuery(Name = "vnp_SecureHash")]
        public string? SecureHash { get; set; }
    }
}
