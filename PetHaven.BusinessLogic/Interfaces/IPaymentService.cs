using PetHaven.BusinessLogic.DTOs;
using PetHaven.Data.Model;

namespace PetHaven.BusinessLogic.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> InitializePayment(OrderPaymentRequestDto request);
        Task<PaymentVerificationResponseDto> VerifyPayment(string reference);
    }
}
