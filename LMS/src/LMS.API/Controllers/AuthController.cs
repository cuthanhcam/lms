using LMS.API.Extensions;
using LMS.Application.DTOs.Auth;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    /// <summary>
    /// Controller handling Authentication and Authorization functionalities
    /// Includes: register, login
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Constructor inject dependencies
        /// </summary>
        /// <param name="authService">Service handling business logic for Authentication</param>
        /// <param name="logger">Logger for logging</param>
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// Register new account
        /// </summary>
        /// <param name="request">Registration information (UserName, Email, Password, Role)</param>
        /// <returns>User information and JWT token</returns>
        /// <response code="200">Registration successful, returns user info and token</response>
        /// <response code="400">Validation failed or email already exists</response>
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInformation("Register attempt for email: {Email}", request.Email);

            var response = await _authService.RegisterAsync(request);

            _logger.LogInformation("User registered successfully: {UserId}", response.UserId);

            return Ok(response);
        }

        /// <summary>
        /// Login to the system
        /// </summary>
        /// <param name="request">Login information (Email, Password)</param>
        /// <returns>User information and JWT token</returns>
        /// <response code="200">Login successful</response>
        /// <response code="401">Invalid email or password</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var response = await _authService.LoginAsync(request);

            _logger.LogInformation("User logged in successfully: {UserId}", response.UserId);

            return Ok(response);
        }
    }
}
