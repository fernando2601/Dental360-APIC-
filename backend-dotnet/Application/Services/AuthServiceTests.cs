using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using DentalSpa.Domain.Entities;
using DentalSpa.Domain.Interfaces;
using DentalSpa.Application.Services;

namespace DentalSpa.Application.Tests
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task LoginAsync_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var mockRepo = new Mock<IAuthRepository>();
            var mockConfig = new Mock<IConfiguration>();
            var mockEmail = new Mock<EmailService>(mockConfig.Object);
            var user = new User { Id = 1, Username = "test", Email = "test@email.com", FullName = "Test User", Role = "user", PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123A") };
            mockRepo.Setup(r => r.ValidateUserCredentialsAsync("test", "password123A")).ReturnsAsync(user);
            mockRepo.Setup(r => r.UpdateLastLoginAsync(user.Id)).ReturnsAsync(true);
            mockRepo.Setup(r => r.CreateSessionAsync(user.Id, It.IsAny<string>(), It.IsAny<System.DateTime>())).ReturnsAsync(true);
            mockConfig.Setup(c => c["Jwt:Key"]).Returns("test-key-test-key-test-key-test-key-test-key-test-key");
            var service = new AuthService(mockRepo.Object, mockConfig.Object, mockEmail.Object);

            // Act
            var result = await service.LoginAsync(new LoginRequest { Username = "test", Password = "password123A" });

            // Assert
            Assert.NotNull(result.Token);
            Assert.Equal("test", result.User.Username);
        }
    }
} 