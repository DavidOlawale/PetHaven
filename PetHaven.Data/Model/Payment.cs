using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Model
{
    public class Payment: BaseEntity
    {
        public string Reference { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
        public string? GatewayResponse { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? VerifiedAt { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }
    }

    public enum PaymentStatus
    {
        Pending,
        Successful,
        Failed
    }
}
