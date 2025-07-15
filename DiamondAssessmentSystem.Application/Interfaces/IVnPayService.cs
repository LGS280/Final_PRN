using DiamondAssessmentSystem.Application.DTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondAssessmentSystem.Application.Interfaces
{
    public interface IVnPayService
    {
        Task<string> CreatePatmentUrl(HttpContext content, VnPaymentRequestDto paymentRequestModel);
        VnPaymentResponseDto ExecutePayment(VnPaymentResponseFromFe request);
        VnPaymentRequestDto CreateVnpayModel(VnPaymentRequestDto paymentRequest);
    }
}
