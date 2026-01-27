using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLMS.API.Services;
using SimpleLMS.Application.DTOs.Users;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Application.Interfaces.Services;

namespace SimpleLMS.API.Controllers
{
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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register([FromBody] CreateUserDto createUserDto)
        {
            var result = await _userService.CreateAsync(createUserDto);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetById(Guid id)
        {
            var result = await _userService.GetByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var result = await _userService.GetAllAsync();
            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> Update(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            var result = await _userService.UpdateAsync(id, updateUserDto);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

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
