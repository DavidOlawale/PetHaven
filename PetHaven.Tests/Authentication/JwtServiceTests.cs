using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moq;
using PetHaven.Authentication;
using PetHaven.Data.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetHaven.Tests.Authentication
{
    public class JwtServiceTests
    {
        private readonly JwtOptions _jwtOptions;
        private readonly JwtService _service;
        private readonly User _testUser;

        public JwtServiceTests()
        {
            // Setup JWT options
            _jwtOptions = new JwtOptions
            {
                Key = "very_secure_secret_key_at_least_16_characters_long",
                Issuer = "PetHaven.API",
                Audience = "PetHaven.Client"
            };

            var mockOptions = new Mock<IOptions<JwtOptions>>();
            mockOptions.Setup(o => o.Value).Returns(_jwtOptions);

            _service = new JwtService(mockOptions.Object);

            // Create test user
            _testUser = new User
            {
                Id = 1,
                Email = "test@example.com",
                FirstName = "Test",
                LastName = "User",
                Role = "PetOwner"
            };
        }

        [Fact]
        public void GenerateToken_ReturnsValidToken()
        {
            // Act
            var token = _service.GenerateToken(_testUser);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public void GenerateToken_ContainsUserClaimsAndCorrectExpiration()
        {
            // Act
            var token = _service.GenerateToken(_testUser);

            // Decode the token to validate its contents
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert - Check claims
            var idClaim = Assert.Single(jwtToken.Claims, c => c.Type == "nameid");
            Assert.Equal(_testUser.Id.ToString(), idClaim.Value);

            var emailClaim = Assert.Single(jwtToken.Claims, c => c.Type == "email");
            Assert.Equal(_testUser.Email, emailClaim.Value);

            var roleClaim = Assert.Single(jwtToken.Claims, c => c.Type == "role");
            Assert.Equal(_testUser.Role, roleClaim.Value);

            // Assert - Check expiration
            // The token should expire in 6 hours (with small tolerance for test execution time)
            var expectedExpiration = DateTime.UtcNow.AddHours(6);
            Assert.True((expectedExpiration - jwtToken.ValidTo).TotalMinutes < 1);
        }

        [Fact]
        public void GenerateToken_ContainsCorrectIssuerAndAudience()
        {
            // Act
            var token = _service.GenerateToken(_testUser);

            // Decode the token to validate its contents
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            // Assert
            Assert.Equal(_jwtOptions.Issuer, jwtToken.Issuer);
            Assert.Equal(_jwtOptions.Audience, jwtToken.Audiences.First());
        }

        [Fact]
        public void GenerateToken_CanBeValidated()
        {
            // Act
            var token = _service.GenerateToken(_testUser);

            // Attempt to validate the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.Key)),
                ValidateIssuer = true,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            // Assert - This will throw if token is invalid
            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            // Additional assertions on the validated token
            Assert.IsType<JwtSecurityToken>(validatedToken);

            // Check claims from the principal
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Assert.Equal(_testUser.Id.ToString(), userId);
        }

        [Fact]
        public void GenerateToken_WithDifferentUsers_ProducesDifferentTokens()
        {
            // Arrange
            var user1 = new User
            {
                Id = 1,
                Email = "user1@example.com",
                Role = "PetOwner"
            };

            var user2 = new User
            {
                Id = 2,
                Email = "user2@example.com",
                Role = "Veterinarian"
            };

            // Act
            var token1 = _service.GenerateToken(user1);
            var token2 = _service.GenerateToken(user2);

            // Assert
            Assert.NotEqual(token1, token2);
        }
    }
}