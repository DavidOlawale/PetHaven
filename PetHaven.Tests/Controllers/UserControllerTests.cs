using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetHaven.BusinessLogic.DTOs.User;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Controllers;
using PetHaven.Data.Model;

namespace PetHaven.Tests.Controllers
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _mockUserService;
        private readonly UserController _controller;
        private readonly int _testUserId = 5;

        public UserControllerTests()
        {
            _mockUserService = new Mock<IUserService>();
            _controller = new UserController(_mockUserService.Object);

            // Simulate authenticated user
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
        public async Task GetUser_ReturnsOkWithUser_WhenUserExists()
        {
            // Arrange
            int userId = 5; // This should match the user ID in the claims
            var expectedUser = new User
            {
                Id = userId,
                FirstName = "Ola",
                LastName = "Dave",
                Email = "olawaledavid11@gmail.com"
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(okResult.Value);
            Assert.Equal(expectedUser, returnedUser);

            // Verify that the service was called with the authenticated user's ID, not the path parameter
            _mockUserService.Verify(s => s.GetUserByIdAsync(_testUserId), Times.Once);
        }

        [Fact]
        public async Task GetUser_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 5;

            _mockUserService.Setup(s => s.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetUser(userId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNoContent_WhenUpdateSucceeds()
        {
            // Arrange
            int userId = 5;
            var updateDto = new UpdateUserDTO
            {
                FirstName = "Ola Updated",
                LastName = "Dave Updated"
            };

            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateUser(updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateUser_ReturnsNotFound_WhenUpdateFails()
        {
            // Arrange
            int userId = 999;
            var updateDto = new UpdateUserDTO
            {
                FirstName = "Nonexistent",
                LastName = "User"
            };

            _mockUserService.Setup(s => s.UpdateUserAsync(userId, updateDto))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateUser(updateDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsUser_WhenUserExists()
        {
            // Arrange
            var expectedUser = new User
            {
                Id = _testUserId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            _mockUserService.Setup(s => s.GetUserByIdAsync(_testUserId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedUser, result);

            // Verify that GetCurrentUserId was used to get the authenticated user's ID
            _mockUserService.Verify(s => s.GetUserByIdAsync(_testUserId), Times.Once);
        }

        [Fact]
        public async Task GetCurrentUser_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            _mockUserService.Setup(s => s.GetUserByIdAsync(_testUserId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.GetCurrentUser();

            // Assert
            Assert.Null(result);
        }
    }
}