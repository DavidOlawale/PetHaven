using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Controllers;
using PetHaven.Data.Model;
using Xunit;

namespace PetHaven.Tests.Controllers
{
    public class ForumsControllerTests
    {
        private readonly Mock<IForumService> _mockForumService;
        private readonly ForumsController _controller;
        private readonly int _testUserId = 5;

        public ForumsControllerTests()
        {
            _mockForumService = new Mock<IForumService>();
            _controller = new ForumsController(_mockForumService.Object);

            // simulate authenticated user
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            // Setup ControllerContext with HttpContext
            var httpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetAllThreads_ReturnsOkWithThreads()
        {
            // Arrange
            var expectedThreads = new List<ForumThread>
            {
                new ForumThread {
                    Id = 1,
                    Title = "Thread 1",
                    Content = "Content 1",
                    UserId = 1,
                    CreatedDate = DateTime.UtcNow
                },
                new ForumThread {
                    Id = 2,
                    Title = "Thread 2",
                    Content = "Content 2",
                    UserId = 2,
                    CreatedDate = DateTime.UtcNow
                }
            };

            _mockForumService.Setup(s => s.GetAllThreadsAsync())
                .ReturnsAsync(expectedThreads);

            // Act
            var result = await _controller.GetAllThreads();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedThreads = Assert.IsAssignableFrom<IEnumerable<ForumThread>>(okResult.Value);
            Assert.Equal(expectedThreads, returnedThreads);
        }

        [Fact]
        public async Task GetThreadById_ReturnsOkWithThread_WhenThreadExists()
        {
            // Arrange
            int threadId = 1;
            var expectedThread = new ForumThread
            {
                Id = threadId,
                Title = "Test Thread",
                Content = "Test Content",
                UserId = 1,
                CreatedDate = DateTime.UtcNow
            };

            _mockForumService.Setup(s => s.GetThreadByIdAsync(threadId))
                .ReturnsAsync(expectedThread);

            // Act
            var result = await _controller.GetThreadById(threadId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedThread = Assert.IsType<ForumThread>(okResult.Value);
            Assert.Equal(expectedThread, returnedThread);
        }

        [Fact]
        public async Task GetThreadById_ReturnsNotFound_WhenThreadDoesNotExist()
        {
            // Arrange
            int threadId = 999;

            _mockForumService.Setup(s => s.GetThreadByIdAsync(threadId))
                .ReturnsAsync((ForumThread)null);

            // Act
            var result = await _controller.GetThreadById(threadId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateThread_ReturnsCreatedAtActionWithThread()
        {
            // Arrange
            var threadDto = new CreateThreadDto
            {
                Title = "New Thread",
                Content = "Thread content",
                Tags = "pets,dogs"
            };

            var createdThread = new ForumThread
            {
                Id = 3,
                Title = threadDto.Title,
                Content = threadDto.Content,
                Tags = threadDto.Tags,
                UserId = _testUserId,
                CreatedDate = DateTime.UtcNow
            };

            _mockForumService.Setup(s => s.CreateThreadAsync(It.Is<CreateThreadDto>(
                dto => dto.Title == threadDto.Title &&
                       dto.Content == threadDto.Content &&
                       dto.Tags == threadDto.Tags &&
                       dto.UserId == _testUserId)))
                .ReturnsAsync(createdThread);

            // Act
            var result = await _controller.CreateThread(threadDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(nameof(ForumsController.GetThreadById), createdAtActionResult.ActionName);
            Assert.Equal(createdThread.Id, createdAtActionResult.RouteValues!["id"]);

            var returnedThread = Assert.IsType<ForumThread>(createdAtActionResult.Value);
            Assert.Equal(createdThread, returnedThread);

            // Verify that GetCurrentUserId was used to set the user ID
            _mockForumService.Verify(s => s.CreateThreadAsync(
                It.Is<CreateThreadDto>(dto => dto.UserId == _testUserId)),
                Times.Once);
        }

        [Fact]
        public async Task AddComment_ReturnsOkWithComment()
        {
            // Arrange
            var commentDto = new CreateCommentDto
            {
                ForumThreadId = 1,
                Content = "New comment"
            };

            var createdComment = new ForumComment
            {
                Id = 5,
                ForumThreadId = commentDto.ForumThreadId,
                Content = commentDto.Content,
                UserId = _testUserId,
                CreatedDate = DateTime.UtcNow
            };

            _mockForumService.Setup(s => s.AddCommentAsync(It.Is<CreateCommentDto>(
                dto => dto.ForumThreadId == commentDto.ForumThreadId &&
                       dto.Content == commentDto.Content &&
                       dto.UserId == _testUserId)))
                .ReturnsAsync(createdComment);

            // Act
            var result = await _controller.AddComment(commentDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedComment = Assert.IsType<ForumComment>(okResult.Value);
            Assert.Equal(createdComment, returnedComment);

            // Verify that GetCurrentUserId was used to set the user ID
            _mockForumService.Verify(s => s.AddCommentAsync(
                It.Is<CreateCommentDto>(dto => dto.UserId == _testUserId)),
                Times.Once);
        }

        [Fact]
        public async Task GetThreadComments_ReturnsOkWithComments()
        {
            // Arrange
            int threadId = 1;
            var expectedComments = new List<ForumComment>
            {
                new ForumComment {
                    Id = 1,
                    ForumThreadId = threadId,
                    Content = "Comment 1",
                    UserId = 1,
                    CreatedDate = DateTime.UtcNow
                },
                new ForumComment {
                    Id = 2,
                    ForumThreadId = threadId,
                    Content = "Comment 2",
                    UserId = 2,
                    CreatedDate = DateTime.UtcNow
                }
            };

            _mockForumService.Setup(s => s.GetThreadCommentsAsync(threadId))
                .ReturnsAsync(expectedComments);

            // Act
            var result = await _controller.GetThreadComments(threadId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedComments = Assert.IsAssignableFrom<IEnumerable<ForumComment>>(okResult.Value);
            Assert.Equal(expectedComments, returnedComments);
        }
    }
}