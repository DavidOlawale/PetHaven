using Microsoft.AspNetCore.Mvc;
using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Controllers;
using PetHaven.Data.Model;
using Xunit;

namespace PetHaven.Tests.Controllers
{
    public class ChatBotControllerTests
    {
        private readonly Mock<IChatHistoryService> _mockChatHistoryService;
        private readonly ChatBotController _controller;

        public ChatBotControllerTests()
        {
            _mockChatHistoryService = new Mock<IChatHistoryService>();
            _controller = new ChatBotController(_mockChatHistoryService.Object);
        }

        [Fact]
        public async Task GetUserChatHistories_ReturnsOkWithHistories()
        {
            // Arrange
            int userId = 1;
            var expectedHistories = new List<ChatBotHistory>
            {
                new ChatBotHistory { Id = 1, UserId = userId, SessionId = "session1" },
                new ChatBotHistory { Id = 2, UserId = userId, SessionId = "session2" }
            };

            _mockChatHistoryService.Setup(s => s.GetUserChatHistoriesAsync(userId))
                .ReturnsAsync(expectedHistories);

            // Act
            var result = await _controller.GetUserChatHistories(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedHistories = Assert.IsType<List<ChatBotHistory>>(okResult.Value);
            Assert.Equal(expectedHistories.Count, returnedHistories.Count);
            Assert.Equal(expectedHistories, returnedHistories);
        }

        [Fact]
        public async Task GetUserChatHistories_ReturnsEmptyList_WhenNoHistoriesExist()
        {
            // Arrange
            int userId = 1;
            var emptyList = new List<ChatBotHistory>();

            _mockChatHistoryService.Setup(s => s.GetUserChatHistoriesAsync(userId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _controller.GetUserChatHistories(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedHistories = Assert.IsType<List<ChatBotHistory>>(okResult.Value);
            Assert.Empty(returnedHistories);
        }

        [Fact]
        public async Task GetChatHistoryBySessionId_ReturnsOkWithHistory_WhenHistoryExists()
        {
            // Arrange
            string sessionId = "test-session-123";
            var expectedHistory = new ChatBotHistory { Id = 1, UserId = 1, SessionId = sessionId };

            _mockChatHistoryService.Setup(s => s.GetChatHistoryBySessionIdAsync(sessionId))
                .ReturnsAsync(expectedHistory);

            // Act
            var result = await _controller.GetChatHistoryBySessionId(sessionId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedHistory = Assert.IsType<ChatBotHistory>(okResult.Value);
            Assert.Equal(expectedHistory, returnedHistory);
        }

        [Fact]
        public async Task GetChatHistoryBySessionId_ReturnsNotFound_WhenHistoryDoesNotExist()
        {
            // Arrange
            string sessionId = "nonexistent-session";

            _mockChatHistoryService.Setup(s => s.GetChatHistoryBySessionIdAsync(sessionId))
                .ReturnsAsync((ChatBotHistory)null);

            // Act
            var result = await _controller.GetChatHistoryBySessionId(sessionId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task StartNewChatSession_ReturnsOkWithNewHistory()
        {
            // Arrange
            int userId = 1;
            var newHistory = new ChatBotHistory
            {
                Id = 1,
                UserId = userId,
                SessionId = "new-session-123",
                StartedAt = DateTime.UtcNow
            };

            _mockChatHistoryService.Setup(s => s.StartNewChatSessionAsync(userId))
                .ReturnsAsync(newHistory);

            // Act
            var result = await _controller.StartNewChatSession(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedHistory = Assert.IsType<ChatBotHistory>(okResult.Value);
            Assert.Equal(newHistory, returnedHistory);
        }

        [Fact]
        public async Task SaveChatMessage_ReturnsOkWithUpdatedHistory_WhenSessionExists()
        {
            // Arrange
            var messageDto = new ChatbotMessageDTO
            {
                SessionId = "existing-session",
                Content = "Hello bot!",
                IsUser = true
            };

            var updatedHistory = new ChatBotHistory
            {
                Id = 1,
                UserId = 1,
                SessionId = messageDto.SessionId,
                Messages = new List<ChatBotMessage>
                {
                    new ChatBotMessage
                    {
                        Content = messageDto.Content,
                        IsUser = messageDto.IsUser,
                        Timestamp = DateTime.UtcNow
                    }
                }
            };

            _mockChatHistoryService.Setup(s => s.SaveChatMessageAsync(
                messageDto.SessionId, messageDto.Content, messageDto.IsUser))
                .ReturnsAsync(updatedHistory);

            // Act
            var result = await _controller.SaveChatMessage(messageDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedHistory = Assert.IsType<ChatBotHistory>(okResult.Value);
            Assert.Equal(updatedHistory, returnedHistory);
        }

        [Fact]
        public async Task SaveChatMessage_ReturnsNotFound_WhenSessionDoesNotExist()
        {
            // Arrange
            var messageDto = new ChatbotMessageDTO
            {
                SessionId = "nonexistent-session",
                Content = "Hello bot!",
                IsUser = true
            };

            _mockChatHistoryService.Setup(s => s.SaveChatMessageAsync(
                messageDto.SessionId, messageDto.Content, messageDto.IsUser))
                .ReturnsAsync((ChatBotHistory)null);

            // Act
            var result = await _controller.SaveChatMessage(messageDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Chat session not found", notFoundResult.Value);
        }

        [Fact]
        public async Task EndChatSession_ReturnsOk_WhenSessionExists()
        {
            // Arrange
            string sessionId = "existing-session";

            _mockChatHistoryService.Setup(s => s.EndChatSessionAsync(sessionId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.EndChatSession(sessionId);

            // Assert
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task EndChatSession_ReturnsNotFound_WhenSessionDoesNotExist()
        {
            // Arrange
            string sessionId = "nonexistent-session";

            _mockChatHistoryService.Setup(s => s.EndChatSessionAsync(sessionId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.EndChatSession(sessionId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Chat session not found", notFoundResult.Value);
        }
    }
}