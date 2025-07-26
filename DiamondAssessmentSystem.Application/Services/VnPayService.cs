using DiamondAssessmentSystem.Application.Bank;
using DiamondAssessmentSystem.Application.DTO;
using DiamondAssessmentSystem.Application.Interfaces;
using DiamondAssessmentSystem.Infrastructure.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DiamondAssessmentSystem.Application.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;
        private readonly IOrderRepository _orderRepo;
        private readonly ICurrentUserService _currentUser;

        public VnPayService(IConfiguration config, IOrderRepository orderRepo, ICurrentUserService currentUser)
        {
            _config = config;
            _orderRepo = orderRepo;
            _currentUser = currentUser;
        }

        public async Task<string> CreatePaymentUrl(HttpContext context, VnPaymentRequestDto request)
        {
            var txnRef = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            var orderId = await _orderRepo.GetCurentOrderId(_currentUser.UserId);

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", ((int)(request.Amount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", request.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", $"requestId={request.RequestId};serviceId={request.ServiceId};amount={request.Amount}");
            vnpay.AddRequestData("vnp_OrderType", "order");
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", txnRef);

            return vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
        }

        public VnPaymentResponseDto ExecutePayment(VnPaymentResponseFromFe response)
        {
            var vnpay = new VnPayLibrary();
            var responseData = new Dictionary<string, string>
        {
            { "vnp_Amount", response.Amount ?? "" },
            { "vnp_BankCode", response.BankCode ?? "" },
            { "vnp_BankTranNo", response.BankTranNo ?? "" },
            { "vnp_CardType", response.CardType ?? "" },
            { "vnp_OrderInfo", response.OrderInfo ?? "" },
            { "vnp_PayDate", response.PayDate ?? "" },
            { "vnp_ResponseCode", response.VnPayResponseCode ?? "" },
            { "vnp_TmnCode", response.TmnCode ?? "" },
            { "vnp_TransactionNo", response.TransactionNo ?? "" },
            { "vnp_TransactionStatus", response.TransactionStatus ?? "" },
            { "vnp_TxnRef", response.TxnRef ?? "" },
            { "vnp_SecureHash", response.SecureHash ?? "" }
        };

            foreach (var kv in responseData)
                vnpay.AddResponseData(kv.Key, kv.Value);

            if (!vnpay.ValidateSignature(response.SecureHash ?? "", _config["VnPay:HashSecret"]))
            {
                return new VnPaymentResponseDto
                {
                    Success = false,
                    VnPayResponseCode = "InvalidSignature",
                    Message = "Invalid signature from VNPAY."
                };
            }

            if (response.TransactionStatus != "00")
            {
                return new VnPaymentResponseDto
                {
                    Success = false,
                    VnPayResponseCode = response.VnPayResponseCode,
                    Message = "Transaction failed at VNPAY."
                };
            }

            var orderInfo = ParseOrderInfo(response.OrderInfo ?? "");
            if (!orderInfo.TryGetValue("requestId", out var reqId) ||
                !orderInfo.TryGetValue("serviceId", out var svcId) ||
                !orderInfo.TryGetValue("amount", out var amt))
            {
                return new VnPaymentResponseDto
                {
                    Success = false,
                    VnPayResponseCode = "MissingOrderInfo",
                    Message = "Missing order info from VNPAY response."
                };
            }

            return new VnPaymentResponseDto
            {
                Success = true,
                OrderDescription = response.OrderInfo,
                OrderId = int.TryParse(reqId, out var orderId) ? orderId : null,
                PaymentMethod = "VnPay",
                TransactionId = response.TransactionNo,
                Token = response.SecureHash,
                VnPayResponseCode = response.VnPayResponseCode
            };
        }

        private Dictionary<string, string> ParseOrderInfo(string orderInfo)
        {
            return orderInfo
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Split('='))
                .Where(arr => arr.Length == 2)
                .ToDictionary(arr => arr[0].Trim(), arr => arr[1].Trim());
        }
    }
}
