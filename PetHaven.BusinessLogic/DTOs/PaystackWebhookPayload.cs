using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.DTOs
{
    public class PaystackWebhookPayload
    {
        public string Event { get; set; }
        public WebhookData Data { get; set; }
    }

    public class WebhookData
    {
        public string Reference { get; set; }
        public string Status { get; set; }
        // ... other webhook properties
    }
}
