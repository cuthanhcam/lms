using LMS.Application.DTOs.Auth;
using LMS.Application.Exceptions;
using LMS.Application.Interfaces;
using LMS.Application.Interfaces.Identity;
using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.ValueObjects;
using LMS.Shared.Constants;

namespace LMS.Application.Services
{
    /// <summary>
    /// Service handling business logic for Authentication and Authorization
    /// - Register new user
    /// - Login
    /// - Generate JWT token
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        /// <summary>
        /// Constructor inject dependencies
        /// </summary>
        /// <param name="unitOfWork">Unit of Work pattern to access repositories</param>
        /// <param name="passwordHasher">Service to hash and verify password</param>
        /// <param name="tokenService">Service to generate JWT token</param>
        public AuthService(
            IUnitOfWork unitOfWork,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Register new user to the system
        /// Business rules:
        /// - Email must be unique
        /// - Role must be valid (Admin, Instructor, Student)
        /// - Password must be hashed before saving
        /// </summary>
        /// <param name="request">Registration information: UserName, Email, Password, Role</param>
        /// <returns>AuthResponse containing user information and JWT token</returns>
        /// <exception cref="BadRequestException">When email already exists or role is invalid</exception>
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            // Validate email uniqueness
            if (!await _unitOfWork.Users.IsEmailUniqueAsync(request.Email))
            {
                throw new BadRequestException("Email already exists");
            }

            // Validate role - Must be one of allowed roles
            if (!Roles.All.Contains(request.Role))
            {
                throw new BadRequestException($"Invalid role. Must be one of: {string.Join(", ", Roles.All)}");
            }

            // Create Email value object
            var emailValueObject = Email.Create(request.Email);
            
            // Parse role string to UserRole enum
            var userRole = request.Role.ToLower() switch
            {
                "admin" => UserRole.Admin,
                "instructor" => UserRole.Instructor,
                "student" => UserRole.Student,
                _ => UserRole.Student // Default to Student if invalid
            };
            
            // Create user entity with hashed password using factory method
            var user = User.Create(
                userName: request.UserName,
                email: emailValueObject,
                passwordHash: _passwordHasher.HashPassword(request.Password), // Hash password before saving
                role: userRole
            );

            // Save user to database
            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            // Generate JWT token so user can authenticate immediately
            var token = _tokenService.GenerateToken(user);

            // Return user information and token
            return new AuthResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email.Value,
                Role = user.Role.ToString(),
                Token = token
            };
        }

        /// <summary>
        /// Login to the system
        /// Business rules:
        /// - Email must exist
        /// - Password must be correct
        /// - User must be active
        /// </summary>
        /// <param name="request">Login information: Email, Password</param>
        /// <returns>AuthResponse containing user information and JWT token</returns>
        /// <exception cref="UnauthorizedException">When email/password is wrong or account is inactive</exception>
        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            // Find user by email
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);

            // Verify email and password
            if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
            {
                throw new UnauthorizedException("Invalid email or password");
            }

            // Check account active
            if (!user.IsActive)
            {
                throw new UnauthorizedException("Account is inactive");
            }

            // Generate JWT token
            var token = _tokenService.GenerateToken(user);

            return new AuthResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email.Value,
                Role = user.Role.ToString(),
                Token = token
            };
        }
    }
}
