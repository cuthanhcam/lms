using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLMS.Application.DTOs.Lessons;
using SimpleLMS.Application.Interfaces.Services;
using System.Security.Claims;

namespace SimpleLMS.API.Controllers
{
    /// <summary>
    /// Controller for managing lessons within courses.
    /// </summary>
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

        /// <summary>
        /// Retrieves a specific lesson by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the lesson.</param>
        /// <returns>The lesson details.</returns>
        /// <response code="200">Returns the lesson details.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the lesson is not found.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _lessonService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves all lessons for a specific course, ordered by their sequence.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course.</param>
        /// <returns>A list of lessons for the specified course.</returns>
        /// <response code="200">Returns the list of lessons.</response>
        /// <response code="400">If the course ID is invalid.</response>
        [HttpGet("course/{courseId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByCourse(Guid courseId)
        {
            var result = await _lessonService.GetLessonsByCourseAsync(courseId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        /// <summary>
        /// Creates a new lesson in a specific course. Only accessible by the course instructor or administrators.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course.</param>
        /// <param name="dto">The lesson creation details.</param>
        /// <returns>The newly created lesson.</returns>
        /// <response code="201">Returns the newly created lesson.</response>
        /// <response code="400">If the lesson data is invalid or user doesn't have permission.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an instructor or administrator.</response>
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

        /// <summary>
        /// Updates an existing lesson. Only accessible by the course instructor or administrators.
        /// </summary>
        /// <param name="id">The unique identifier of the lesson.</param>
        /// <param name="dto">The updated lesson information.</param>
        /// <returns>The updated lesson details.</returns>
        /// <response code="200">Returns the updated lesson.</response>
        /// <response code="400">If the update data is invalid or user doesn't have permission.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an instructor or administrator.</response>
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

        /// <summary>
        /// Soft deletes a lesson. Only accessible by the course instructor or administrators.
        /// </summary>
        /// <param name="id">The unique identifier of the lesson to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the lesson was successfully deleted.</response>
        /// <response code="400">If the deletion failed or user doesn't have permission.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an instructor or administrator.</response>
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
