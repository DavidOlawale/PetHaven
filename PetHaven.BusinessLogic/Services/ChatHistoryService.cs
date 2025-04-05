using Microsoft.AspNetCore.Identity;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.BusinessLogic.Services
{
    public class ChatHistoryService : IChatHistoryService
    {
        private readonly IChatBotRepository _chatbotRepository;

        public ChatHistoryService(IChatBotRepository chatHistoryRepository)
        {
            _chatbotRepository = chatHistoryRepository;
        }

        public async Task<List<ChatBotHistory>> GetUserChatHistoriesAsync(int userId)
        {
            return await _chatbotRepository.GetUserChatHistoriesAsync(userId);
        }

        public async Task<ChatBotHistory?> GetChatHistoryBySessionIdAsync(string sessionId)
        {
            return await _chatbotRepository.GetChatHistoryBySessionIdAsync(sessionId);
        }

        public async Task<ChatBotHistory> StartNewChatSessionAsync(int userId)
        {
            var chatHistory = new ChatBotHistory
            {
                UserId = userId,
                SessionId = Guid.NewGuid().ToString(),
                StartedAt = DateTime.UtcNow
            };

            return await _chatbotRepository.CreateChatHistoryAsync(chatHistory);
        }

        public async Task<ChatBotHistory> SaveChatMessageAsync(string sessionId, string content, bool isUser)
        {
            var message = new ChatBotMessage
            {
                Content = content,
                IsUser = isUser,
                Timestamp = DateTime.UtcNow,
                
            };

            return await _chatbotRepository.AddMessageToChatHistoryAsync(sessionId, message);
        }

        public async Task<bool> EndChatSessionAsync(string sessionId)
        {
            return await _chatbotRepository.EndChatSessionAsync(sessionId);
        }
    }
}
