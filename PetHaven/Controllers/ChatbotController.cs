using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Data.Model;

namespace PetHaven.Controllers
{
    [ApiController]
    [Route("api/chatbot")]
    [Authorize]
    public class ChatBotController : ControllerBase
    {
        private readonly IChatHistoryService _chatHistoryService;

        public ChatBotController(IChatHistoryService chatHistoryService)
        {
            _chatHistoryService = chatHistoryService;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<ChatBotHistory>>> GetUserChatHistories(int userId)
        {
            var histories = await _chatHistoryService.GetUserChatHistoriesAsync(userId);
            return Ok(histories);
        }

        [HttpGet("session/{sessionId}")]
        public async Task<ActionResult<ChatBotHistory>> GetChatHistoryBySessionId(string sessionId)
        {
            var history = await _chatHistoryService.GetChatHistoryBySessionIdAsync(sessionId);
            if (history == null)
                return NotFound();

            return Ok(history);
        }

        [HttpPost("start/{userId}")]
        public async Task<ActionResult<ChatBotHistory>> StartNewChatSession(int userId)
        {
            var history = await _chatHistoryService.StartNewChatSessionAsync(userId);
            return Ok(history);
        }

        [HttpPost("message")]
        public async Task<ActionResult<ChatBotHistory>> SaveChatMessage([FromBody] ChatbotMessageDTO request)
        {
            var history = await _chatHistoryService.SaveChatMessageAsync(
                request.SessionId, request.Content, request.IsUser);

            if (history == null)
                return NotFound("Chat session not found");

            return Ok(history);
        }

        [HttpPost("end/{sessionId}")]
        public async Task<ActionResult> EndChatSession(string sessionId)
        {
            var success = await _chatHistoryService.EndChatSessionAsync(sessionId);
            if (!success)
                return NotFound("Chat session not found");

            return Ok();
        }
    }    
}