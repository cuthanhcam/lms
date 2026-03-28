using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using LMS.API.IntegrationTests.Fixtures;
using LMS.Application.DTOs.Auth;
using Xunit;

namespace LMS.API.IntegrationTests.Controllers
{
    /// <summary>
    /// Integration tests for AuthController
    /// Test end-to-end flow: HTTP Request → Controller → Service → Database → Response
    /// </summary>
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public AuthControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        #region Register Tests

        /// <summary>
        /// Test: Register with valid information should return 200 OK and token
        /// </summary>
        [Fact]
        public async Task Register_WithValidRequest_ShouldReturn200WithToken()
        {
            // Arrange
            var request = new RegisterRequest
            {
                UserName = "newuser",
                Email = "newuser@test.com",
                Password = "Test@123",
                Role = "Student"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            result.Should().NotBeNull();
            result!.Email.Should().Be(request.Email);
            result.UserName.Should().Be(request.UserName);
            result.Role.Should().Be(request.Role);
            result.Token.Should().NotBeNullOrEmpty();
            result.UserId.Should().NotBeEmpty();
        }

        /// <summary>
        /// Test: Register with existing email should return 400 Bad Request
        /// </summary>
        [Fact]
        public async Task Register_WithDuplicateEmail_ShouldReturn400()
        {
            // Arrange - Register first user
            var firstRequest = new RegisterRequest
            {
                UserName = "user1",
                Email = "duplicate@test.com",
                Password = "Test@123",
                Role = "Student"
            };

            await _client.PostAsJsonAsync("/api/auth/register", firstRequest);

            // Register second user with the same email
            var secondRequest = new RegisterRequest
            {
                UserName = "user2",
                Email = "duplicate@test.com", // Duplicate email
                Password = "Test@456",
                Role = "Instructor"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", secondRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Test: Register with weak password (not meeting requirements) should return 400
        /// </summary>
        [Fact]
        public async Task Register_WithWeakPassword_ShouldReturn400()
        {
            // Arrange
            var request = new RegisterRequest
            {
                UserName = "testuser",
                Email = "test@weak.com",
                Password = "123", // Password too short and does not meet complexity rules
                Role = "Student"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Test: Register with invalid role should return 400
        /// </summary>
        [Fact]
        public async Task Register_WithInvalidRole_ShouldReturn400()
        {
            // Arrange
            var request = new RegisterRequest
            {
                UserName = "testuser",
                Email = "test@role.com",
                Password = "Test@123",
                Role = "InvalidRole" // Role does not exist
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        /// <summary>
        /// Test: Register with missing fields should return 400
        /// </summary>
        [Fact]
        public async Task Register_WithMissingFields_ShouldReturn400()
        {
            // Arrange - Empty email
            var request = new RegisterRequest
            {
                UserName = "testuser",
                Email = "", // Missing email
                Password = "Test@123",
                Role = "Student"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region Login Tests

        /// <summary>
        /// Test: Login with valid credentials should return 200 OK and token
        /// </summary>
        [Fact]
        public async Task Login_WithValidCredentials_ShouldReturn200WithToken()
        {
            // Arrange - Register user first
            var registerRequest = new RegisterRequest
            {
                UserName = "loginuser",
                Email = "login@test.com",
                Password = "Login@123",
                Role = "Student"
            };

            await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

            // Login with the credentials from registration
            var loginRequest = new LoginRequest
            {
                Email = "login@test.com",
                Password = "Login@123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
            result.Should().NotBeNull();
            result!.Email.Should().Be(loginRequest.Email);
            result.Token.Should().NotBeNullOrEmpty();
        }

        /// <summary>
        /// Test: Login with non-existent email should return 401 Unauthorized
        /// </summary>
        [Fact]
        public async Task Login_WithNonExistentEmail_ShouldReturn401()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "notexist@test.com",
                Password = "Test@123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Test: Login with wrong password should return 401 Unauthorized
        /// </summary>
        [Fact]
        public async Task Login_WithWrongPassword_ShouldReturn401()
        {
            // Arrange - Register user first
            var registerRequest = new RegisterRequest
            {
                UserName = "wrongpwduser",
                Email = "wrongpwd@test.com",
                Password = "Correct@123",
                Role = "Student"
            };

            await _client.PostAsJsonAsync("/api/auth/register", registerRequest);

            // Login with wrong password
            var loginRequest = new LoginRequest
            {
                Email = "wrongpwd@test.com",
                Password = "Wrong@123" // Wrong password
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        /// <summary>
        /// Test: Login with missing fields should return 400
        /// </summary>
        [Fact]
        public async Task Login_WithMissingFields_ShouldReturn400()
        {
            // Arrange
            var loginRequest = new LoginRequest
            {
                Email = "", // Missing email
                Password = "Test@123"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        #endregion

        #region JWT Token Tests

        /// <summary>
        /// Test: Received JWT token should be usable for authentication
        /// </summary>
        [Fact]
        public async Task Register_TokenShouldBeValidForAuthentication()
        {
            // Arrange - Register user
            var registerRequest = new RegisterRequest
            {
                UserName = "tokenuser",
                Email = "token@test.com",
                Password = "Token@123",
                Role = "Student"
            };

            var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
            var authResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

            // Act - Use token to call protected endpoint
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult!.Token);

            var protectedResponse = await _client.GetAsync("/api/my-enrollments");

            // Assert - Should not return 401 Unauthorized
            protectedResponse.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        }

        #endregion
    }
}
