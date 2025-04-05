using Microsoft.EntityFrameworkCore;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PetHaven.Data.Repositories
{
    public class ChatBotHistoryRepository : IChatBotRepository
    {
        private readonly AppDbContext _context;

        public ChatBotHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatBotHistory>> GetUserChatHistoriesAsync(int userId)
        {
            return await _context.ChatBotHistories
                .Where(ch => ch.UserId == userId)
                .Include(ch => ch.Messages)
                .OrderByDescending(ch => ch.StartedAt)
                .ToListAsync();
        }

        public async Task<ChatBotHistory?> GetChatHistoryBySessionIdAsync(string sessionId)
        {
            return await _context.ChatBotHistories
                .Include(ch => ch.Messages.OrderBy(m => m.Timestamp))
                .FirstOrDefaultAsync(ch => ch.SessionId == sessionId);
        }

        public async Task<ChatBotHistory> CreateChatHistoryAsync(ChatBotHistory chatHistory)
        {
            chatHistory.StartedAt = DateTime.UtcNow;
            _context.ChatBotHistories.Add(chatHistory);
            await _context.SaveChangesAsync();
            return chatHistory;
        }

        public async Task<ChatBotHistory> AddMessageToChatHistoryAsync(string sessionId, ChatBotMessage message)
        {
            var chatHistory = await GetChatHistoryBySessionIdAsync(sessionId);
            if (chatHistory == null)
                return null;

            message.Timestamp = DateTime.UtcNow;
            chatHistory.Messages.Add(message);
            await _context.SaveChangesAsync();
            return chatHistory;
        }

        public async Task<bool> EndChatSessionAsync(string sessionId)
        {
            var chatHistory = await GetChatHistoryBySessionIdAsync(sessionId);
            if (chatHistory == null)
                return false;

            chatHistory.EndedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}