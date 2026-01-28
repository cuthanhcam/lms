using System.Net.Http.Headers;
using System.Net.Http.Json;
using LMS.API.IntegrationTests.Fixtures;
using LMS.Application.DTOs.Auth;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.API.IntegrationTests.Helpers
{
    /// <summary>
    /// Helper class to create authenticated HTTP client for integration tests
    /// </summary>
    public class AuthenticatedClientHelper
    {
        private readonly CustomWebApplicationFactory _factory;

        public AuthenticatedClientHelper(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// T?o HTTP client ?� authenticated v?i role Admin
        /// </summary>
        public async Task<HttpClient> CreateAdminClientAsync()
        {
            return await CreateAuthenticatedClientAsync("Admin", "admin@lms.com", "Admin@123");
        }

        /// <summary>
        /// T?o HTTP client ?� authenticated v?i role Instructor
        /// </summary>
        public async Task<HttpClient> CreateInstructorClientAsync()
        {
            return await CreateAuthenticatedClientAsync("Instructor", "instructor@lms.com", "Instructor@123");
        }

        /// <summary>
        /// T?o HTTP client ?� authenticated v?i role Student
        /// </summary>
        public async Task<HttpClient> CreateStudentClientAsync()
        {
            return await CreateAuthenticatedClientAsync("Student", "student@lms.com", "Student@123");
        }

        /// <summary>
        /// T?o HTTP client ?� authenticated v?i custom role
        /// </summary>
        private async Task<HttpClient> CreateAuthenticatedClientAsync(string role, string email, string password)
        {
            var client = _factory.CreateClient();

            // Register user
            var registerRequest = new RegisterRequest
            {
                UserName = role.ToLower() + "_user",
                Email = email,
                Password = password,
                Role = role
            };

            // Clear database before register to avoid duplicate
            using (var scope = _factory.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var existingUser = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (existingUser != null)
                {
                    db.Users.Remove(existingUser);
                    await db.SaveChangesAsync();
                }
            }

            var registerResponse = await client.PostAsJsonAsync("/api/auth/register", registerRequest);

            if (!registerResponse.IsSuccessStatusCode)
            {
                // N?u register fail, th? login
                var loginRequest = new LoginRequest
                {
                    Email = email,
                    Password = password
                };

                var loginResponse = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
                loginResponse.EnsureSuccessStatusCode();

                var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResult!.Token);
            }
            else
            {
                var registerResult = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", registerResult!.Token);
            }

            return client;
        }

        /// <summary>
        /// T?o HTTP client kh�ng authenticated (anonymous)
        /// </summary>
        public HttpClient CreateAnonymousClient()
        {
            return _factory.CreateClient();
        }
    }
}
