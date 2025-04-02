using Microsoft.AspNetCore.Http;
using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.DTOs
{
    public class OrderPaymentRequestDto
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        [StringLength(3)]
        public string Currency { get; set; } = "USD";
    }
}
