using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.DTOs
{
    public class PaymentVerificationResponseDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public decimal Amount { get; set; }
        public string Reference { get; set; }
        public string GatewayResponse { get; set; }
    }
}
