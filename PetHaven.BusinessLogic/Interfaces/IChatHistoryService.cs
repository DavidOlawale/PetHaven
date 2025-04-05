using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Interfaces
{
    public interface IChatHistoryService
    {
        Task<List<ChatBotHistory>> GetUserChatHistoriesAsync(int userId);
        Task<ChatBotHistory?> GetChatHistoryBySessionIdAsync(string sessionId);
        Task<ChatBotHistory> StartNewChatSessionAsync(int userId);
        Task<ChatBotHistory> SaveChatMessageAsync(string sessionId, string content, bool isUser);
        Task<bool> EndChatSessionAsync(string sessionId);
    }
}
