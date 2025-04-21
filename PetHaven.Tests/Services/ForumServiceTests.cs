using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Services;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;

namespace PetHaven.Tests.Services
{
    public class ForumServiceTests
    {
        private readonly Mock<IForumRepository> _mockForumRepository;
        private readonly ForumService _service;

        public ForumServiceTests()
        {
            _mockForumRepository = new Mock<IForumRepository>();
            _service = new ForumService(_mockForumRepository.Object);
        }

        [Fact]
        public async Task GetAllThreadsAsync_ReturnsAllThreads()
        {
            // Arrange
            var expectedThreads = new List<ForumThread>
            {
                new ForumThread
                {
                    Id = 1,
                    Title = "Thread 1",
                    Content = "Content 1",
                    UserId = 1,
                    CreatedDate = DateTime.UtcNow.AddDays(-1),
                    Tags = "tag1,tag2"
                },
                new ForumThread
                {
                    Id = 2,
                    Title = "Thread 2",
                    Content = "Content 2",
                    UserId = 2,
                    CreatedDate = DateTime.UtcNow,
                    Tags = "tag2,tag3"
                }
            };

            _mockForumRepository.Setup(r => r.GetAllForumThreadsAsync())
                .ReturnsAsync(expectedThreads);

            // Act
            var result = await _service.GetAllThreadsAsync();

            // Assert
            Assert.Equal(expectedThreads, result);
            _mockForumRepository.Verify(r => r.GetAllForumThreadsAsync(), Times.Once);
        }

        [Fact]
        public async Task GetThreadByIdAsync_ReturnsThread_WhenThreadExists()
        {
            // Arrange
            int threadId = 1;
            var expectedThread = new ForumThread
            {
                Id = threadId,
                Title = "Test Thread",
                Content = "Test Content",
                UserId = 1,
                CreatedDate = DateTime.UtcNow.AddDays(-1),
                Tags = "test,thread"
            };

            _mockForumRepository.Setup(r => r.GetForumThreadByIdAsync(threadId))
                .ReturnsAsync(expectedThread);

            // Act
            var result = await _service.GetThreadByIdAsync(threadId);

            // Assert
            Assert.Equal(expectedThread, result);
            _mockForumRepository.Verify(r => r.GetForumThreadByIdAsync(threadId), Times.Once);
        }

        [Fact]
        public async Task GetThreadByIdAsync_ReturnsNull_WhenThreadDoesNotExist()
        {
            // Arrange
            int threadId = 999;

            _mockForumRepository.Setup(r => r.GetForumThreadByIdAsync(threadId))
                .ReturnsAsync((ForumThread)null);

            // Act
            var result = await _service.GetThreadByIdAsync(threadId);

            // Assert
            Assert.Null(result);
            _mockForumRepository.Verify(r => r.GetForumThreadByIdAsync(threadId), Times.Once);
        }

        [Fact]
        public async Task CreateThreadAsync_CreatesAndReturnsThread()
        {
            // Arrange
            var threadDto = new CreateThreadDto
            {
                Title = "New Thread",
                Content = "New Thread Content",
                Tags = "new,thread",
                UserId = 1
            };

            ForumThread capturedThread = null;
            var createdThread = new ForumThread
            {
                Id = 3,
                Title = threadDto.Title,
                Content = threadDto.Content,
                Tags = threadDto.Tags,
                UserId = threadDto.UserId,
                CreatedDate = DateTime.UtcNow
            };

            _mockForumRepository.Setup(r => r.AddForumThreadAsync(It.IsAny<ForumThread>()))
                .Callback<ForumThread>(t => capturedThread = t)
                .ReturnsAsync(createdThread);

            // Act
            var result = await _service.CreateThreadAsync(threadDto);

            // Assert
            Assert.Equal(createdThread, result);

            // Verify thread was created with correct properties
            Assert.NotNull(capturedThread);
            Assert.Equal(threadDto.Title, capturedThread.Title);
            Assert.Equal(threadDto.Content, capturedThread.Content);
            Assert.Equal(threadDto.Tags, capturedThread.Tags);
            Assert.Equal(threadDto.UserId, capturedThread.UserId);

            // Check that CreatedDate is within the last few seconds
            var timeDiff = DateTime.UtcNow - capturedThread.CreatedDate;
            Assert.True(timeDiff.TotalSeconds < 5);

            _mockForumRepository.Verify(r => r.AddForumThreadAsync(It.IsAny<ForumThread>()), Times.Once);
        }

        [Fact]
        public async Task AddCommentAsync_CreatesAndReturnsComment()
        {
            // Arrange
            var commentDto = new CreateCommentDto
            {
                Content = "New Comment",
                ForumThreadId = 1,
                UserId = 2
            };

            ForumComment capturedComment = null;
            var createdComment = new ForumComment
            {
                Id = 5,
                Content = commentDto.Content,
                ForumThreadId = commentDto.ForumThreadId,
                UserId = commentDto.UserId,
                CreatedDate = DateTime.UtcNow
            };

            _mockForumRepository.Setup(r => r.AddForumCommentAsync(It.IsAny<ForumComment>()))
                .Callback<ForumComment>(c => capturedComment = c)
                .ReturnsAsync(createdComment);

            // Act
            var result = await _service.AddCommentAsync(commentDto);

            // Assert
            Assert.Equal(createdComment, result);

            // Verify comment was created with correct properties
            Assert.NotNull(capturedComment);
            Assert.Equal(commentDto.Content, capturedComment.Content);
            Assert.Equal(commentDto.ForumThreadId, capturedComment.ForumThreadId);
            Assert.Equal(commentDto.UserId, capturedComment.UserId);

            // Check that CreatedDate is within the last few seconds
            var timeDiff = DateTime.UtcNow - capturedComment.CreatedDate;
            Assert.True(timeDiff.TotalSeconds < 5);

            _mockForumRepository.Verify(r => r.AddForumCommentAsync(It.IsAny<ForumComment>()), Times.Once);
        }

        [Fact]
        public async Task GetThreadCommentsAsync_ReturnsCommentsForThread()
        {
            // Arrange
            int threadId = 1;
            var expectedComments = new List<ForumComment>
            {
                new ForumComment
                {
                    Id = 1,
                    Content = "Comment 1",
                    ForumThreadId = threadId,
                    UserId = 2,
                    CreatedDate = DateTime.UtcNow.AddHours(-2)
                },
                new ForumComment
                {
                    Id = 2,
                    Content = "Comment 2",
                    ForumThreadId = threadId,
                    UserId = 3,
                    CreatedDate = DateTime.UtcNow.AddHours(-1)
                }
            };

            _mockForumRepository.Setup(r => r.GetForumCommentsByThreadIdAsync(threadId))
                .ReturnsAsync(expectedComments);

            // Act
            var result = await _service.GetThreadCommentsAsync(threadId);

            // Assert
            Assert.Equal(expectedComments, result);
            _mockForumRepository.Verify(r => r.GetForumCommentsByThreadIdAsync(threadId), Times.Once);
        }

        [Fact]
        public async Task GetThreadCommentsAsync_ReturnsEmptyList_WhenNoCommentsExist()
        {
            // Arrange
            int threadId = 999;
            var emptyList = new List<ForumComment>();

            _mockForumRepository.Setup(r => r.GetForumCommentsByThreadIdAsync(threadId))
                .ReturnsAsync(emptyList);

            // Act
            var result = await _service.GetThreadCommentsAsync(threadId);

            // Assert
            Assert.Empty(result);
            _mockForumRepository.Verify(r => r.GetForumCommentsByThreadIdAsync(threadId), Times.Once);
        }
    }
}