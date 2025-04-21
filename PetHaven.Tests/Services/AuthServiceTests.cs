using Microsoft.AspNetCore.Identity;
using Moq;
using PetHaven.BusinessLogic.DTOs;
using PetHaven.BusinessLogic.Interfaces;
using PetHaven.BusinessLogic.Services;
using PetHaven.Data.Model;
using PetHaven.Data.Repositories.Interfaces;

namespace PetHaven.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockEmailService = new Mock<IEmailService>();
            _authService = new AuthService(_mockUserRepository.Object, _mockEmailService.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsTrue_WhenCredentialsAreValid()
        {
            // Arrange
            string email = "test@example.com";
            string password = "password123";
            var user = new User
            {
                Email = email,
                PasswordHash = "---"
            };

            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(email))
                .ReturnsAsync(user);

            // Create a partial mock to override the VerifyPasswordHash method
            var mockAuthService = new Mock<AuthService>(_mockUserRepository.Object, _mockEmailService.Object)
            {
                CallBase = true
            };
            mockAuthService.Setup(s => s.VerifyPasswordHash(user, password, user.PasswordHash))
                .Returns(true);

            // Act
            var result = await mockAuthService.Object.AuthenticateAsync(email, password);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsFalse_WhenUserDoesNotExist()
        {
            // Arrange
            string email = "nonexistent@example.com";
            string password = "password123";

            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.AuthenticateAsync(email, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AuthenticateAsync_ReturnsFalse_WhenPasswordIsIncorrect()
        {
            // Arrange
            string email = "test@example.com";
            string password = "wrongpassword";
            var user = new User
            {
                Email = email,
                PasswordHash = "hashedPassword"
            };

            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(email))
                .ReturnsAsync(user);

            // Create a partial mock to override the VerifyPasswordHash method
            var mockAuthService = new Mock<AuthService>(_mockUserRepository.Object, _mockEmailService.Object)
            {
                CallBase = true
            };
            mockAuthService.Setup(s => s.VerifyPasswordHash(user, password, user.PasswordHash))
                .Returns(false);

            // Act
            var result = await mockAuthService.Object.AuthenticateAsync(email, password);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RegisterAsync_CreatesNewPetOwner_WhenDataIsValid()
        {
            // Arrange
            var signUpDto = new SignUpDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password123",
                ZipCode = "12345",
                Role = UserRoles.PetOwner
            };

            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(signUpDto.Email))
                .ReturnsAsync((User)null);

            User savedUser = null;
            _mockUserRepository.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);

            // Create a partial mock to override the CreatePasswordHash method
            var mockAuthService = new Mock<AuthService>(_mockUserRepository.Object, _mockEmailService.Object)
            {
                CallBase = true
            };
            mockAuthService.Setup(s => s.CreatePasswordHash(It.IsAny<User>(), signUpDto.Password))
                .Returns("HashedPassword123");

            // Act
            var result = await mockAuthService.Object.RegisterAsync(signUpDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(signUpDto.FirstName, result.FirstName);
            Assert.Equal(signUpDto.LastName, result.LastName);
            Assert.Equal(signUpDto.Email, result.Email);
            Assert.Equal(signUpDto.ZipCode, result.ZipCode);
            Assert.Equal(signUpDto.Role, result.Role);
            Assert.Equal("HashedPassword123", result.PasswordHash);

            Assert.NotNull(savedUser);
            _mockEmailService.Verify(e => e.SendSignupConfirmationAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_CreatesNewVeterinarian_WhenDataIsValid()
        {
            // Arrange
            var signUpDto = new SignUpDTO
            {
                FirstName = "Jane",
                LastName = "Smith",
                Email = "jane.smith@example.com",
                Password = "password123",
                ZipCode = "54321",
                Role = UserRoles.Veterinarian,
                VeterinarianDetails = new VeterinarianDetails
                {
                    LicenseNumber = "VET12345",
                    ClinicName = "Pet Care Clinic",
                    Specialization = "Small Animals"
                }
            };

            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(signUpDto.Email))
                .ReturnsAsync((User)null);

            User savedUser = null;
            _mockUserRepository.Setup(r => r.AddUserAsync(It.IsAny<User>()))
                .Callback<User>(u => savedUser = u)
                .Returns(Task.CompletedTask);

            // Create a partial mock to override the CreatePasswordHash method
            var mockAuthService = new Mock<AuthService>(_mockUserRepository.Object, _mockEmailService.Object)
            {
                CallBase = true
            };
            mockAuthService.Setup(s => s.CreatePasswordHash(It.IsAny<User>(), signUpDto.Password))
                .Returns("HashedPassword123");

            // Act
            var result = await mockAuthService.Object.RegisterAsync(signUpDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(signUpDto.FirstName, result.FirstName);
            Assert.Equal(signUpDto.LastName, result.LastName);
            Assert.Equal(signUpDto.Email, result.Email);
            Assert.Equal(signUpDto.ZipCode, result.ZipCode);
            Assert.Equal(signUpDto.Role, result.Role);
            Assert.Equal("HashedPassword123", result.PasswordHash);

            // Verify veterinarian-specific properties
            Assert.Equal(signUpDto.VeterinarianDetails.LicenseNumber, result.LicenseNumber);
            Assert.Equal(signUpDto.VeterinarianDetails.ClinicName, result.ClinicName);
            Assert.Equal(signUpDto.VeterinarianDetails.Specialization, result.Specialization);

            Assert.NotNull(savedUser);
            _mockEmailService.Verify(e => e.SendSignupConfirmationAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ThrowsException_WhenEmailAlreadyExists()
        {
            // Arrange
            var signUpDto = new SignUpDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "existing@example.com",
                Password = "password123",
                ZipCode = "12345",
                Role = UserRoles.PetOwner
            };

            var existingUser = new User
            {
                Email = signUpDto.Email
            };

            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(signUpDto.Email))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(signUpDto));
            Assert.Equal("Email already exists", exception.Message);

            _mockUserRepository.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
            _mockEmailService.Verify(e => e.SendSignupConfirmationAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_ThrowsException_WhenRoleIsInvalid()
        {
            // Arrange
            var signUpDto = new SignUpDTO
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Password = "password123",
                ZipCode = "12345",
                Role = "abc" // Invalid role value
            };

            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(signUpDto.Email))
                .ReturnsAsync((User)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(signUpDto));
            Assert.Equal("Invalid role name", exception.Message);

            _mockUserRepository.Verify(r => r.AddUserAsync(It.IsAny<User>()), Times.Never);
            _mockEmailService.Verify(e => e.SendSignupConfirmationAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task CheckEmailExists_ReturnsTrue_WhenEmailExists()
        {
            // Arrange
            string email = "existing@example.com";
            var user = new User { Email = email };

            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(email))
                .ReturnsAsync(user);

            // Act
            var result = await _authService.CheckEmailExists(email);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckEmailExists_ReturnsFalse_WhenEmailDoesNotExist()
        {
            // Arrange
            string email = "nonexistent@example.com";

            _mockUserRepository.Setup(r => r.GetUserByEmailAsync(email))
                .ReturnsAsync((User)null);

            // Act
            var result = await _authService.CheckEmailExists(email);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CreatePasswordHash_ReturnsHashedPassword()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            string password = "password123";

            // Act
            var result = _authService.CreatePasswordHash(user, password);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(password, result); // Ensure the password was actually hashed
        }

        [Fact]
        public void VerifyPasswordHash_ReturnsTrue_WhenPasswordIsCorrect()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            string password = "password123";

            // Generate an actual hash using the PasswordHasher
            var passwordHasher = new PasswordHasher<User>();
            string passwordHash = passwordHasher.HashPassword(user, password);

            // Act
            var result = _authService.VerifyPasswordHash(user, password, passwordHash);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPasswordHash_ReturnsFalse_WhenPasswordIsIncorrect()
        {
            // Arrange
            var user = new User { Email = "test@example.com" };
            string correctPassword = "password123";
            string wrongPassword = "wrongpassword";

            // Generate a hash for the correct password
            var passwordHasher = new PasswordHasher<User>();
            string passwordHash = passwordHasher.HashPassword(user, correctPassword);

            // Act
            var result = _authService.VerifyPasswordHash(user, wrongPassword, passwordHash);

            // Assert
            Assert.False(result);
        }
    }
}