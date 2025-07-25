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
        private readonly IOrderRepository _orderRepository;
        private readonly ICurrentUserService _currentUser;

        public VnPayService(IConfiguration config, IOrderRepository orderRepository, ICurrentUserService currentUser)
        {
            _config = config;
            _orderRepository = orderRepository;
            _currentUser = currentUser;
        }

        public async Task<string> CreatePatmentUrl(HttpContext content, VnPaymentRequestDto paymentRequestModel)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var orderId = await _orderRepository.GetCurentOrderId(_currentUser.UserId);
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (paymentRequestModel.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không 
            //mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND
            //(một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần(khử phần thập phân), sau đó gửi sang VNPAY
            //là: 10000000

            vnpay.AddRequestData("vnp_CreateDate", paymentRequestModel.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(content));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderId);
            vnpay.AddRequestData("vnp_OrderType", "order"); 
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick); 
            var paymnentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);
            return paymnentUrl;
        }

        public VnPaymentRequestDto CreateVnpayModel(VnPaymentRequestDto input)
        {
            if (input == null || input.Amount <= 0)
                return null;

            return new VnPaymentRequestDto
            {
                RequestId = input.RequestId,
                ServiceId = input.ServiceId,
                Amount = input.Amount,
                CreatedDate = DateTime.Now
            };
        }

        public VnPaymentResponseDto ExecutePayment(VnPaymentResponseFromFe request)
        {
            var vnpay = new VnPayLibrary();
            var responseData = new Dictionary<string, string>
            {
                { "vnp_Amount", request.Amount },
                { "vnp_BankCode", request.BankCode },
                { "vnp_BankTranNo", request.BankTranNo },
                { "vnp_CardType", request.CardType },
                { "vnp_OrderInfo", request.OrderInfo },
                { "vnp_PayDate", request.PayDate },
                { "vnp_ResponseCode", request.VnPayResponseCode },
                { "vnp_TmnCode", request.TmnCode },
                { "vnp_TransactionNo", request.TransactionNo },
                { "vnp_TransactionStatus", request.TransactionStatus },
                { "vnp_TxnRef", request.TxnRef },
                { "vnp_SecureHash", request.SecureHash }
            };

            foreach (var kv in responseData)
            {
                vnpay.AddResponseData(kv.Key, kv.Value);
            }

            var isValidSignature = vnpay.ValidateSignature(request.SecureHash, _config["VnPay:HashSecret"]);
            if (!isValidSignature)
            {
                return new VnPaymentResponseDto
                {
                    Success = false,
                    VnPayResponseCode = "InvalidSignature"
                };
            }

            if (request.TransactionStatus != "00")
            {
                return new VnPaymentResponseDto
                {
                    Success = false,
                    VnPayResponseCode = request.VnPayResponseCode
                };
            }

            // Parse order info
            var orderInfoDict = ParseOrderInfo(request.OrderInfo);
            if (!orderInfoDict.TryGetValue("requestId", out var reqIdStr) ||
                !orderInfoDict.TryGetValue("serviceId", out var svcIdStr) ||
                !orderInfoDict.TryGetValue("amount", out var amountStr))
            {
                return new VnPaymentResponseDto
                {
                    Success = false,
                    VnPayResponseCode = "MissingOrderInfo"
                };
            }

            return new VnPaymentResponseDto
            {
                Success = true,
                PaymentMethod = "VnPay",
                OrderDescription = request.OrderInfo,
                OrderId = int.Parse(reqIdStr),
                TransactionId = request.TransactionNo,
                Token = request.SecureHash,
                VnPayResponseCode = request.VnPayResponseCode
            };
        }

        private Dictionary<string, string> ParseOrderInfo(string orderInfo)
        {
            return orderInfo
                .Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(pair => pair.Split('='))
                .Where(parts => parts.Length == 2)
                .ToDictionary(parts => parts[0].Trim(), parts => parts[1].Trim());
        }
    }
}
