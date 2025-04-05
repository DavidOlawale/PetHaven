using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Data.Model
{
    public class ChatBotHistory: BaseEntity
    {

        public string SessionId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public List<ChatBotMessage> Messages { get; set; } = new List<ChatBotMessage>();

    }
}
