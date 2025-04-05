using Microsoft.AspNetCore.Http;
using PetHaven.Data.Model;
using System;

namespace PetHaven.BusinessLogic.DTOs
{
    public class ChatbotMessageDTO
    {
        public string SessionId { get; set; }
        public string Content { get; set; }
        public bool IsUser { get; set; }
    }
}
