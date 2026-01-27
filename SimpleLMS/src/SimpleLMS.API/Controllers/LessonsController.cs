using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLMS.Application.DTOs.Lessons;
using SimpleLMS.Application.Interfaces.Services;
using System.Security.Claims;

namespace SimpleLMS.API.Controllers
{
    [ApiController]
    [Route("api/lessons")]
    [Authorize]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;

        public LessonsController(ILessonService lessonService)
        {
            _lessonService = lessonService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _lessonService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpGet("course/{courseId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCourse(Guid courseId)
        {
            var result = await _lessonService.GetLessonsByCourseAsync(courseId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpPost("course/{courseId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Create(Guid courseId, [FromBody] CreateLessonDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _lessonService.CreateAsync(courseId, userId, dto);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLessonDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _lessonService.UpdateAsync(id, userId, dto);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _lessonService.DeleteAsync(id, userId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}
