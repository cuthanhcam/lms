using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLMS.API.Services;
using SimpleLMS.Application.DTOs.Users;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Application.Interfaces.Services;

namespace SimpleLMS.API.Controllers
{
    /// <summary>
    /// Controller for managing user authentication and user-related operations.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(
            IUserService userService,
            IJwtTokenService jwtTokenService,
            IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _jwtTokenService = jwtTokenService;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="loginDto">The login credentials containing username and password.</param>
        /// <returns>A JWT token and user information if authentication is successful.</returns>
        /// <response code="200">Returns the JWT token and user details.</response>
        /// <response code="401">If the credentials are invalid.</response>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _userService.LoginAsync(loginDto);

            if (!result.IsSuccess)
                return Unauthorized(new { message = result.ErrorMessage });

            // Get User entity to generate JWT token
            var user = await _unitOfWork.Users.GetByUsernameAsync(loginDto.Username);
            if (user != null)
            {
                result.Data!.Token = _jwtTokenService.GenerateToken(user);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Registers a new user in the system.
        /// </summary>
        /// <param name="createUserDto">The user registration details.</param>
        /// <returns>The newly created user information.</returns>
        /// <response code="201">Returns the newly created user.</response>
        /// <response code="400">If the user data is invalid or username/email already exists.</response>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto createUserDto)
        {
            var result = await _userService.CreateAsync(createUserDto);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        /// <summary>
        /// Retrieves a specific user by their unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the user.</param>
        /// <returns>The user details if found.</returns>
        /// <response code="200">Returns the user details.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the user is not found.</response>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves all users in the system. Only accessible by administrators.
        /// </summary>
        /// <returns>A list of all users.</returns>
        /// <response code="200">Returns the list of all users.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an administrator.</response>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result.Data);
        }

        /// <summary>
        /// Updates an existing user's information.
        /// </summary>
        /// <param name="id">The unique identifier of the user to update.</param>
        /// <param name="updateUserDto">The updated user information.</param>
        /// <returns>The updated user details.</returns>
        /// <response code="200">Returns the updated user details.</response>
        /// <response code="400">If the update data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            var result = await _userService.UpdateAsync(id, updateUserDto);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        /// <summary>
        /// Deletes a user from the system. Only accessible by administrators.
        /// </summary>
        /// <param name="id">The unique identifier of the user to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the user was successfully deleted.</response>
        /// <response code="400">If the deletion failed.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an administrator.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var result = await _userService.DeleteAsync(id);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return NoContent();
        }
    }
}
