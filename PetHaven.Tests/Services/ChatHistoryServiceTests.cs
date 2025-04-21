using Moq;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.BusinessLogic.Services;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;

namespace PetHaven.Tests.Services
{
    public class ChatHistoryServiceTests
    {
        private readonly Mock<IChatBotRepository> _mockChatbotRepository;
        private readonly IChatHistoryService _service;

        public ChatHistoryServiceTests()
        {
            _mockChatbotRepository = new Mock<IChatBotRepository>();
            _service = new ChatHistoryService(_mockChatbotRepository.Object);
        }

        [Fact]
        public async Task GetUserChatHistoriesAsync_ReturnsRepositoryResult()
        {
            // Arrange
            int userId = 1;
            var expectedHistories = new List<ChatBotHistory>
            {
                new ChatBotHistory
                {
                    Id = 1,
                    UserId = userId,
                    SessionId = "session1",
                    StartedAt = DateTime.UtcNow.AddDays(-1)
                },
                new ChatBotHistory
                {
                    Id = 2,
                    UserId = userId,
                    SessionId = "session2",
                    StartedAt = DateTime.UtcNow
                }
            };

            _mockChatbotRepository.Setup(r => r.GetUserChatHistoriesAsync(userId))
                .ReturnsAsync(expectedHistories);

            // Act
            var result = await _service.GetUserChatHistoriesAsync(userId);

            // Assert
            Assert.Equal(expectedHistories, result);
            _mockChatbotRepository.Verify(r => r.GetUserChatHistoriesAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetChatHistoryBySessionIdAsync_ReturnsRepositoryResult()
        {
            // Arrange
            string sessionId = "test-session-id";
            var expectedHistory = new ChatBotHistory
            {
                Id = 1,
                UserId = 1,
                SessionId = sessionId,
                StartedAt = DateTime.UtcNow.AddDays(-1)
            };

            _mockChatbotRepository.Setup(r => r.GetChatHistoryBySessionIdAsync(sessionId))
                .ReturnsAsync(expectedHistory);

            // Act
            var result = await _service.GetChatHistoryBySessionIdAsync(sessionId);

            // Assert
            Assert.Equal(expectedHistory, result);
            _mockChatbotRepository.Verify(r => r.GetChatHistoryBySessionIdAsync(sessionId), Times.Once);
        }

        [Fact]
        public async Task GetChatHistoryBySessionIdAsync_ReturnsNull_WhenSessionDoesNotExist()
        {
            // Arrange
            string sessionId = "nonexistent-session-id";

            _mockChatbotRepository.Setup(r => r.GetChatHistoryBySessionIdAsync(sessionId))
                .ReturnsAsync((ChatBotHistory)null);

            // Act
            var result = await _service.GetChatHistoryBySessionIdAsync(sessionId);

            // Assert
            Assert.Null(result);
            _mockChatbotRepository.Verify(r => r.GetChatHistoryBySessionIdAsync(sessionId), Times.Once);
        }

        [Fact]
        public async Task StartNewChatSessionAsync_CreatesAndReturnsNewSession()
        {
            // Arrange
            int userId = 1;
            ChatBotHistory capturedHistory = null;

            _mockChatbotRepository.Setup(r => r.CreateChatHistoryAsync(It.IsAny<ChatBotHistory>()))
                .Callback<ChatBotHistory>(h => capturedHistory = h)
                .ReturnsAsync((ChatBotHistory h) => h);

            // Act
            var result = await _service.StartNewChatSessionAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.NotNull(result.SessionId);
            Assert.NotEqual(string.Empty, result.SessionId);

            // Verify the history was created with correct properties
            Assert.NotNull(capturedHistory);
            Assert.Equal(userId, capturedHistory.UserId);
            Assert.Equal(result.SessionId, capturedHistory.SessionId);

            // Check that the StartedAt is within the last few seconds
            var timeDiff = DateTime.UtcNow - capturedHistory.StartedAt;
            Assert.True(timeDiff.TotalSeconds < 5);

            _mockChatbotRepository.Verify(r => r.CreateChatHistoryAsync(It.IsAny<ChatBotHistory>()), Times.Once);
        }

        [Fact]
        public async Task SaveChatMessageAsync_AddsMessageToSession()
        {
            // Arrange
            string sessionId = "test-session-id";
            string content = "Hello, this is a test message";
            bool isUser = true;

            var expectedHistory = new ChatBotHistory
            {
                Id = 1,
                UserId = 1,
                SessionId = sessionId,
                StartedAt = DateTime.UtcNow.AddDays(-1)
            };

            ChatBotMessage capturedMessage = null;

            _mockChatbotRepository.Setup(r => r.AddMessageToChatHistoryAsync(
                    sessionId, It.IsAny<ChatBotMessage>()))
                .Callback<string, ChatBotMessage>((sid, m) => capturedMessage = m)
                .ReturnsAsync(expectedHistory);

            // Act
            var result = await _service.SaveChatMessageAsync(sessionId, content, isUser);

            // Assert
            Assert.Equal(expectedHistory, result);

            // Verify the message was created with correct properties
            Assert.NotNull(capturedMessage);
            Assert.Equal(content, capturedMessage.Content);
            Assert.Equal(isUser, capturedMessage.IsUser);

            // Check that the Timestamp is within the last few seconds
            var timeDiff = DateTime.UtcNow - capturedMessage.Timestamp;
            Assert.True(timeDiff.TotalSeconds < 5);

            _mockChatbotRepository.Verify(r => r.AddMessageToChatHistoryAsync(
                sessionId, It.IsAny<ChatBotMessage>()), Times.Once);
        }

        [Fact]
        public async Task EndChatSessionAsync_ReturnsRepositoryResult()
        {
            // Arrange
            string sessionId = "test-session-id";
            bool expectedResult = true;

            _mockChatbotRepository.Setup(r => r.EndChatSessionAsync(sessionId))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _service.EndChatSessionAsync(sessionId);

            // Assert
            Assert.Equal(expectedResult, result);
            _mockChatbotRepository.Verify(r => r.EndChatSessionAsync(sessionId), Times.Once);
        }

        [Fact]
        public async Task EndChatSessionAsync_ReturnsFalse_WhenSessionDoesNotExist()
        {
            // Arrange
            string sessionId = "nonexistent-session-id";
            bool expectedResult = false;

            _mockChatbotRepository.Setup(r => r.EndChatSessionAsync(sessionId))
                .ReturnsAsync(expectedResult);

            // Act
            var result = await _service.EndChatSessionAsync(sessionId);

            // Assert
            Assert.False(result);
            _mockChatbotRepository.Verify(r => r.EndChatSessionAsync(sessionId), Times.Once);
        }
    }
}