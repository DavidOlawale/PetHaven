using System.ComponentModel.DataAnnotations;

namespace PetHaven.Configurations
{
    public class PaystackConfig
    {
        public string SecretKey { get; set; }

        public string WebhookSecret { get; set; }

    }
}
