using Moq;
using PetHaven.BusinessLogic.DTOs.User;
using PetHaven.BusinessLogic.Services;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;

namespace PetHaven.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _service = new UserService(_mockUserRepository.Object);
        }

        [Fact]
        public void GetAllUsers_ReturnsAllUsers()
        {
            // Arrange
            var expectedUsers = new List<User>
            {
                new User { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com" },
                new User { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com" }
            };

            _mockUserRepository.Setup(r => r.GetAllUsers())
                .Returns(expectedUsers);

            // Act
            var result = _service.GetAllUsers();

            // Assert
            Assert.Equal(expectedUsers, result);
            _mockUserRepository.Verify(r => r.GetAllUsers(), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            int userId = 1;
            var expectedUser = new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com"
            };

            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.Equal(expectedUser, result);
            _mockUserRepository.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 999;

            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _service.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(result);
            _mockUserRepository.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ReturnsUser_WhenUserExists()
        {
            // Arrange
            string email = "john@example.com";
            var expectedUser = new User
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = email
            };

            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(email))
                .ReturnsAsync(expectedUser);

            // Act
            var result = await _service.GetUserByEmailAsync(email);

            // Assert
            Assert.Equal(expectedUser, result);
            _mockUserRepository.Verify(r => r.GetUserByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task GetUserByEmailAsync_ReturnsNull_WhenUserDoesNotExist()
        {
            // Arrange
            string email = "nonexistent@example.com";

            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _service.GetUserByEmailAsync(email);

            // Assert
            Assert.Null(result);
            _mockUserRepository.Verify(r => r.GetUserByEmailAsync(email), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsTrue_AndUpdatesUser_WhenUserExists()
        {
            // Arrange
            int userId = 1;
            var existingUser = new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                ZipCode = "12345"
            };

            var updateDto = new UpdateUserDTO
            {
                FirstName = "Johnny",
                LastName = "Doe",
                ZipCode = "54321"
            };

            User capturedUser = null;
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateUserAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);

            // Verify user was updated with correct properties
            Assert.NotNull(capturedUser);
            Assert.Equal(updateDto.FirstName, capturedUser.FirstName);
            Assert.Equal(updateDto.LastName, capturedUser.LastName);
            Assert.Equal(updateDto.ZipCode, capturedUser.ZipCode);
            Assert.Equal(existingUser.Email, capturedUser.Email); // Email shouldn't change

            _mockUserRepository.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
            _mockUserRepository.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task UpdateUserAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            int userId = 999;
            var updateDto = new UpdateUserDTO
            {
                FirstName = "Johnny",
                LastName = "Doe",
                ZipCode = "54321"
            };

            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var result = await _service.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.False(result);
            _mockUserRepository.Verify(r => r.GetUserByIdAsync(userId), Times.Once);
            _mockUserRepository.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task UpdateUserAsync_PreservesExistingFields_WhenUpdateFieldsAreNull()
        {
            // Arrange
            int userId = 1;
            var existingUser = new User
            {
                Id = userId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                ZipCode = "12345"
            };

            var updateDto = new UpdateUserDTO
            {
                FirstName = null,
                LastName = "Smith", // Only updating last name
                ZipCode = null
            };

            User capturedUser = null;
            _mockUserRepository.Setup(r => r.GetUserByIdAsync(userId))
                .ReturnsAsync(existingUser);
            _mockUserRepository.Setup(r => r.UpdateUserAsync(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.UpdateUserAsync(userId, updateDto);

            // Assert
            Assert.True(result);

            // Verify only last name was updated, other fields preserved
            Assert.NotNull(capturedUser);
            Assert.Equal(existingUser.FirstName, capturedUser.FirstName); // Unchanged
            Assert.Equal(updateDto.LastName, capturedUser.LastName); // Changed
            Assert.Equal(existingUser.ZipCode, capturedUser.ZipCode); // Unchanged
            Assert.Equal(existingUser.Email, capturedUser.Email); // Unchanged

            _mockUserRepository.Verify(r => r.UpdateUserAsync(It.IsAny<User>()), Times.Once);
        }
    }
}