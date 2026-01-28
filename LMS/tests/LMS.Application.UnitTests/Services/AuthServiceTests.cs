using FluentAssertions;
using LMS.Application.DTOs.Auth;
using LMS.Application.Exceptions;
using LMS.Application.Interfaces.Identity;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Services;
using LMS.Application.UnitTests.Builders;
using LMS.Domain.Entities;
using LMS.Shared.Constants;
using Moq;
using Xunit;

namespace LMS.Application.UnitTests.Services
{
    /// <summary>
    /// Unit tests for AuthService
    /// Test scenarios: Register success, Register failed, Login success, Login failed
    /// </summary>
    public class AuthServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            // Setup mocks
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockTokenService = new Mock<ITokenService>();

            // T?o instance c?a AuthService v?i mocked dependencies
            _authService = new AuthService(
                _mockUnitOfWork.Object,
                _mockPasswordHasher.Object,
                _mockTokenService.Object);
        }

        #region Register Tests

        /// <summary>
        /// Test: Register v?i th�ng tin h?p l? ph?i th�nh c�ng
        /// </summary>
        [Fact]
        public async Task RegisterAsync_WithValidRequest_ShouldReturnAuthResponse()
        {
            // Arrange
            var request = new RegisterRequest
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Test@123",
                Role = Roles.Student
            };

            var hashedPassword = "hashedPassword123";
            var token = "jwt.token.here";

            // Setup mocks
            _mockUnitOfWork.Setup(x => x.Users.IsEmailUniqueAsync(request.Email))
                .ReturnsAsync(true);

            _mockPasswordHasher.Setup(x => x.HashPassword(request.Password))
                .Returns(hashedPassword);

            _mockTokenService.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns(token);

            _mockUnitOfWork.Setup(x => x.Users.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _authService.RegisterAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.UserName.Should().Be(request.UserName);
            result.Email.Should().Be(request.Email);
            result.Role.Should().Be(request.Role);
            result.Token.Should().Be(token);

            // Verify mock calls
            _mockUnitOfWork.Verify(x => x.Users.IsEmailUniqueAsync(request.Email), Times.Once);
            _mockPasswordHasher.Verify(x => x.HashPassword(request.Password), Times.Once);
            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Test: Register v?i email ?� t?n t?i ph?i throw BadRequestException
        /// </summary>
        [Fact]
        public async Task RegisterAsync_WithDuplicateEmail_ShouldThrowBadRequestException()
        {
            // Arrange
            var request = new RegisterRequest
            {
                UserName = "testuser",
                Email = "existing@example.com",
                Password = "Test@123",
                Role = Roles.Student
            };

            _mockUnitOfWork.Setup(x => x.Users.IsEmailUniqueAsync(request.Email))
                .ReturnsAsync(false);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(
                () => _authService.RegisterAsync(request));

            _mockUnitOfWork.Verify(x => x.Users.IsEmailUniqueAsync(request.Email), Times.Once);
            _mockPasswordHasher.Verify(x => x.HashPassword(It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Test: Register v?i role kh�ng h?p l? ph?i throw BadRequestException
        /// </summary>
        [Fact]
        public async Task RegisterAsync_WithInvalidRole_ShouldThrowBadRequestException()
        {
            // Arrange
            var request = new RegisterRequest
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Test@123",
                Role = "InvalidRole"
            };

            _mockUnitOfWork.Setup(x => x.Users.IsEmailUniqueAsync(request.Email))
                .ReturnsAsync(true);

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(
                () => _authService.RegisterAsync(request));
        }

        #endregion

        #region Login Tests

        /// <summary>
        /// Test: Login v?i credentials ?�ng ph?i th�nh c�ng
        /// </summary>
        [Fact]
        public async Task LoginAsync_WithValidCredentials_ShouldReturnAuthResponse()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            var user = new UserBuilder()
                .WithEmail(request.Email)
                .WithPasswordHash("hashedPassword")
                .Build();

            var token = "jwt.token.here";

            _mockUnitOfWork.Setup(x => x.Users.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockPasswordHasher.Setup(x => x.VerifyPassword(user.PasswordHash, request.Password))
                .Returns(true);

            _mockTokenService.Setup(x => x.GenerateToken(user))
                .Returns(token);

            // Act
            var result = await _authService.LoginAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Email.Should().Be(request.Email);
            result.Token.Should().Be(token);

            _mockUnitOfWork.Verify(x => x.Users.GetByEmailAsync(request.Email), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(user.PasswordHash, request.Password), Times.Once);
            _mockTokenService.Verify(x => x.GenerateToken(user), Times.Once);
        }

        /// <summary>
        /// Test: Login v?i email kh�ng t?n t?i ph?i throw UnauthorizedException
        /// </summary>
        [Fact]
        public async Task LoginAsync_WithNonExistentEmail_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "notfound@example.com",
                Password = "Test@123"
            };

            _mockUnitOfWork.Setup(x => x.Users.GetByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.LoginAsync(request));

            _mockUnitOfWork.Verify(x => x.Users.GetByEmailAsync(request.Email), Times.Once);
            _mockPasswordHasher.Verify(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        /// <summary>
        /// Test: Login v?i password sai ph?i throw UnauthorizedException
        /// </summary>
        [Fact]
        public async Task LoginAsync_WithWrongPassword_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "WrongPassword"
            };

            var user = new UserBuilder()
                .WithEmail(request.Email)
                .WithPasswordHash("hashedPassword")
                .Build();

            _mockUnitOfWork.Setup(x => x.Users.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockPasswordHasher.Setup(x => x.VerifyPassword(user.PasswordHash, request.Password))
                .Returns(false);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.LoginAsync(request));

            _mockPasswordHasher.Verify(x => x.VerifyPassword(user.PasswordHash, request.Password), Times.Once);
            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<User>()), Times.Never);
        }

        /// <summary>
        /// Test: Login v?i user kh�ng active ph?i throw UnauthorizedException
        /// </summary>
        [Fact]
        public async Task LoginAsync_WithInactiveUser_ShouldThrowUnauthorizedException()
        {
            // Arrange
            var request = new LoginRequest
            {
                Email = "test@example.com",
                Password = "Test@123"
            };

            var user = new UserBuilder()
                .WithEmail(request.Email)
                .AsInactive()
                .Build();

            _mockUnitOfWork.Setup(x => x.Users.GetByEmailAsync(request.Email))
                .ReturnsAsync(user);

            _mockPasswordHasher.Setup(x => x.VerifyPassword(user.PasswordHash, request.Password))
                .Returns(true);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(
                () => _authService.LoginAsync(request));
        }

        #endregion
    }
}
