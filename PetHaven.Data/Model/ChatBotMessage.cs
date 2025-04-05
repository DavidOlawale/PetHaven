using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Model
{
    public class ChatBotMessage: BaseEntity
    {
        public string Content { get; set; }
        public bool IsUser { get; set; } // True if sent by user, else, sent by chatbot
        public DateTime Timestamp { get; set; }

        public int ChatBotHistoryId { get; set; }
    }
}
