using Microsoft.EntityFrameworkCore;
using PetHaven.Data.Model;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetHaven.Data.Repositories.Interfaces
{
    public interface IChatBotRepository
    {
        Task<List<ChatBotHistory>> GetUserChatHistoriesAsync(int userId);
        Task<ChatBotHistory?> GetChatHistoryBySessionIdAsync(string sessionId);
        Task<ChatBotHistory> CreateChatHistoryAsync(ChatBotHistory chatHistory);
        Task<ChatBotHistory> AddMessageToChatHistoryAsync(string sessionId, ChatBotMessage message);
        Task<bool> EndChatSessionAsync(string sessionId);
    }
}