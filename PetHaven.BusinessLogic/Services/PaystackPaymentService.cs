using Microsoft.Extensions.Configuration;
using Paystack.Net.SDK;
using Paystack.Net.SDK.Models;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;

namespace PetHaven.BusinessLogic.Services
{
    public class PaystackPaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly PayStackApi _paystackApi;
        private readonly IConfiguration _config;

        public PaystackPaymentService(
            IPaymentRepository paymentRepo,
            IOrderRepository orderRepo,
            IConfiguration config)
        {
            _paymentRepository = paymentRepo;
            _orderRepository = orderRepo;
            _config = config;
            _paystackApi = new PayStackApi(_config["Paystack:SecretKey"]);
        }

        public async Task<PaymentResponseDto> InitializePayment(OrderPaymentRequestDto request)
        {
            var order = await _orderRepository.GetByIdAsync(request.OrderId);
            if (order == null) throw new Exception("Order not found");
            if (order.Status != OrderStatus.Pending)
                throw new InvalidOperationException("Order payment already initialized");

            var payment = new Payment
            {
                OrderId = order.Id,
                Reference = GeneratePaymentReference(order.Id),
                Amount = order.TotalAmount,
                Currency = request.Currency,
            };

            await _paymentRepository.AddAsync(payment);

            var paystackRequest = new TransactionInitializationRequestModel
            {
                amount = (int)(order.TotalAmount * 100),
                email = order.User.Email,
                reference = payment.Reference,
                callbackUrl = $"{_config["BaseUrl"]}/api/payments/verify"
            };

            //var response = await _paystackApi.Transactions.InitializeTransaction(paystackRequest);

            return new PaymentResponseDto
            {
                //AuthorizationUrl = response.data.authorization_url,
                Reference = payment.Reference
            };
        }

        public async Task<PaymentVerificationResponseDto> VerifyPayment(string reference)
        {
            var payment = await _paymentRepository.GetByReferenceAsync(reference, includeOrder: true);
            if (payment == null) throw new Exception("Payment not found");

            if (payment.Status != PaymentStatus.Pending)
                return MapToDto(payment);

            var response = await _paystackApi.Transactions.VerifyTransaction(reference);

            payment.Status = response.data.status == "success"
                ? PaymentStatus.Successful
                : PaymentStatus.Failed;
            payment.GatewayResponse = response.data.gateway_response;
            payment.VerifiedAt = DateTime.UtcNow;

            if (payment.Status == PaymentStatus.Successful)
            {
                payment.Order.Status = OrderStatus.Processing;
                await _orderRepository.UpdateAsync(payment.Order);
            }

            await _paymentRepository.UpdateAsync(payment);

            return MapToDto(payment);
        }

        private static string GeneratePaymentReference(int orderId)
            => $"PAY-{orderId}-{DateTime.Now.Ticks}--PetHaven";

        private static PaymentVerificationResponseDto MapToDto(Payment payment) => new()
        {
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Reference = payment.Reference,
            GatewayResponse = payment.GatewayResponse!,
            Status = PaymentStatusToString(payment.Status)
        };

        private static string PaymentStatusToString(PaymentStatus status)
        {
            switch (status)
            {
                case PaymentStatus.Pending:
                    return "pending";
                case PaymentStatus.Successful:
                    return "successful";
                case PaymentStatus.Failed:
                    return "failed";
                default:
                    return "";
            }
        }
    }
}