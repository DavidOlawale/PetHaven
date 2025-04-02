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
    public class PaymentResponseDto
    {
        public string AuthorizationUrl { get; set; }
        public string Reference { get; set; }
        public string AccessCode { get; set; }
    }
}
