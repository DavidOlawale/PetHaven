using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using PetHaven.Authentication;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.Controllers;
using PetHaven.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetHaven.Tests.Controllers
{

    public class AuthControllerTests
    {
        private readonly Mock<IAuthService> _mockAuthService;
        private readonly Mock<IUserService> _mockUserService;
        private readonly Mock<IJwtService> _mockJwtService;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            _mockAuthService = new Mock<IAuthService>();
            _mockUserService = new Mock<IUserService>();
            _mockJwtService = new Mock<IJwtService>();
            _controller = new AuthController(_mockAuthService.Object, _mockUserService.Object, _mockJwtService.Object);
        }

        [Fact]
        public async Task Login_WithValidCredentials_ReturnsOkWithToken()
        {
            // Arrange
            var loginDto = new LoginDTO { Email = "test@gmail.com", Password = "password123#" };
            var user = new Data.Model.User { Id = 1, Email = "test@gmail.com", PasswordHash = "ilwhabdou2nf" };
            var token = "jwt-token";

            _mockUserService.Setup(s => s.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);
            _mockAuthService.Setup(s => s.VerifyPasswordHash(user, loginDto.Password, user.PasswordHash))
                .Returns(true);
            _mockJwtService.Setup(s => s.GenerateToken(user))
                .Returns(token);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            //var okResult = Assert.IsType<OkObjectResult>(result);
            //dynamic tokenObject = okResult.Value;
            //Assert.Equal(token, tokenObject?.token);

            var okResult = Assert.IsType<OkObjectResult>(result);

            // Convert to JObject for easier property access
            var resultObject = JObject.FromObject(okResult.Value);
            Assert.Equal(token, resultObject["token"].ToString());
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDTO { Email = "test@example.com", Password = "wrongpassword" };
            var user = new Data.Model.User { Id = 1, Email = "test@example.com", PasswordHash = "ilwhabdou2nf" };

            _mockUserService.Setup(s => s.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync(user);
            _mockAuthService.Setup(s => s.VerifyPasswordHash(user, loginDto.Password, user.PasswordHash))
                .Returns(false);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
        {
            // Arrange
            var loginDto = new LoginDTO { Email = "nknownuser@gmail.com", Password = "password123" };

            _mockUserService.Setup(s => s.GetUserByEmailAsync(loginDto.Email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task SignUp_ReturnsOkWithToken()
        {
            // Arrange
            var signUpDto = new SignUpDTO { Email = "new@example.com", Password = "newpassword123" };
            var newUser = new User { Id = 2, Email = "new@example.com" };
            var token = "new-jwt-token-456";

            _mockAuthService.Setup(s => s.RegisterAsync(signUpDto))
                .ReturnsAsync(newUser);
            _mockJwtService.Setup(s => s.GenerateToken(newUser))
                .Returns(token);

            // Act
            var result = await _controller.SignUp(signUpDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            var resultObject = JObject.FromObject(okResult.Value!);
            Assert.Equal(token, resultObject["token"]!.ToString());
        }

        [Fact]
        public async Task CheckEmailExists_WithExistingEmail_ReturnsTrue()
        {
            // Arrange
            string email = "existing@example.com";

            _mockAuthService.Setup(s => s.CheckEmailExists(email))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.CheckEmailExists(email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckEmailExists_WithNonExistingEmail_ReturnsFalse()
        {
            // Arrange
            string email = "nonexisting@example.com";

            _mockAuthService.Setup(s => s.CheckEmailExists(email))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.CheckEmailExists(email);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckEmailExists_WithEmptyEmail_ReturnsFalse()
        {
            // Arrange
            string email = "";

            // Act
            var result = await _controller.CheckEmailExists(email);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckEmailExists_WithNullEmail_ReturnsFalse()
        {
            // Arrange
            string email = null;

            // Act
            var result = await _controller.CheckEmailExists(email);

            // Assert
            Assert.False(result);
        }
    }
}
