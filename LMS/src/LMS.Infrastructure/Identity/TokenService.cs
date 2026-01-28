using LMS.Application.Interfaces.Identity;
using LMS.Domain.Entities;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace LMS.Infrastructure.Identity
{
    /// <summary>
    /// Service implementation to generate JWT (JSON Web Token)
    /// JWT is used to authenticate and authorize users
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor inject IConfiguration to read JWT settings from appsettings.json
        /// </summary>
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generate JWT token for user
        /// Token contains claims: UserId, UserName, Email, Role
        /// Token has expiration time based on config (default: 60 minutes)
        /// </summary>
        /// <param name="user">User entity to generate token</param>
        /// <returns>JWT token string format: "header.payload.signature"</returns>
        /// <exception cref="InvalidOperationException">When JWT SecretKey is not configured</exception>
        public string GenerateToken(User user)
        {
            // Read JWT settings from appsettings.json
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "60");

            // Create signing key from secret key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create claims - user information in token
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // User ID
                new Claim(ClaimTypes.Name, user.UserName),                // Username
                new Claim(ClaimTypes.Email, user.Email.Value),            // Email value from Email value object
                new Claim(ClaimTypes.Role, user.Role.ToString()),         // Role (Admin/Instructor/Student) as string
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Token ID (unique)
            };

            // Create JWT token
            var token = new JwtSecurityToken(
                issuer: issuer,                                     // Token issuer
                audience: audience,                                 // Token audience
                claims: claims,                                     // Claims
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes), // Expiration time
                signingCredentials: credentials                     // Signature
            );

            // Convert token object to string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

