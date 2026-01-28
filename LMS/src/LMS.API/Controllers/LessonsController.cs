using LMS.API.Extensions;
using LMS.Application.DTOs.Lessons;
using LMS.Application.Interfaces;
using LMS.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    /// <summary>
    /// Controller managing Lessons in Course
    /// Includes: CRUD lessons, role-based access control Instructor/Admin
    /// </summary>
    [ApiController]
    [Route("api")]
    [Produces("application/json")]
    [Authorize]
    public class LessonsController : ControllerBase
    {
        private readonly ILessonService _lessonService;
        private readonly ILogger<LessonsController> _logger;

        /// <summary>
        /// Constructor inject dependencies
        /// </summary>
        public LessonsController(ILessonService lessonService, ILogger<LessonsController> logger)
        {
            _lessonService = lessonService;
            _logger = logger;
        }

        /// <summary>
        /// Get list of lessons in a course
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <returns>List of lessons ordered by Order field</returns>
        /// <response code="200">Returns list of lessons</response>
        /// <response code="404">Course not found</response>
        [HttpGet("courses/{courseId}/lessons")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<LessonDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<LessonDto>>> GetByCourseId(Guid courseId)
        {
            _logger.LogInformation("Getting lessons for course: {CourseId}", courseId);

            var lessons = await _lessonService.GetByCourseIdAsync(courseId);

            return Ok(lessons);
        }

        /// <summary>
        /// Get lesson details
        /// </summary>
        /// <param name="id">Lesson ID</param>
        /// <returns>Lesson details</returns>
        /// <response code="200">Returns lesson details</response>
        /// <response code="404">Lesson not found</response>
        [HttpGet("lessons/{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(LessonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LessonDto>> GetById(Guid id)
        {
            _logger.LogInformation("Getting lesson by ID: {LessonId}", id);

            var lesson = await _lessonService.GetByIdAsync(id);

            return Ok(lesson);
        }

        /// <summary>
        /// Create new lesson in course (Instructor owner or Admin only)
        /// </summary>
        /// <param name="courseId">Course ID</param>
        /// <param name="request">Lesson information: Title, Content, Order</param>
        /// <returns>Created lesson</returns>
        /// <response code="201">Lesson created successfully</response>
        /// <response code="400">Validation failed</response>
        /// <response code="403">No permission (can only add lessons to own courses)</response>
        /// <response code="404">Course not found</response>
        [HttpPost("courses/{courseId}/lessons")]
        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [ProducesResponseType(typeof(LessonDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LessonDto>> Create(Guid courseId, [FromBody] CreateLessonRequest request)
        {
            var userId = User.GetUserId();
            var userRole = User.GetUserRole();

            _logger.LogInformation("User {UserId} creating lesson for course: {CourseId}", userId, courseId);

            var lesson = await _lessonService.CreateAsync(courseId, request, userId, userRole);

            _logger.LogInformation("Lesson created successfully: {LessonId}", lesson.Id);

            return CreatedAtAction(nameof(GetById), new { id = lesson.Id }, lesson);
        }

        /// <summary>
        /// Update lesson (Instructor owner or Admin only)
        /// </summary>
        /// <param name="id">Lesson ID</param>
        /// <param name="request">Update information</param>
        /// <returns>Updated lesson</returns>
        /// <response code="200">Update successful</response>
        /// <response code="400">Validation failed</response>
        /// <response code="403">No permission</response>
        /// <response code="404">Lesson not found</response>
        [HttpPut("lessons/{id}")]
        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [ProducesResponseType(typeof(LessonDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<LessonDto>> Update(Guid id, [FromBody] UpdateLessonRequest request)
        {
            var userId = User.GetUserId();
            var userRole = User.GetUserRole();

            _logger.LogInformation("User {UserId} updating lesson: {LessonId}", userId, id);

            var lesson = await _lessonService.UpdateAsync(id, request, userId, userRole);

            _logger.LogInformation("Lesson updated successfully: {LessonId}", lesson.Id);

            return Ok(lesson);
        }

        /// <summary>
        /// Delete lesson (Instructor owner or Admin only) - Soft delete
        /// </summary>
        /// <param name="id">Lesson ID</param>
        /// <returns>No content</returns>
        /// <response code="204">Delete successful</response>
        /// <response code="403">No permission</response>
        /// <response code="404">Lesson not found</response>
        [HttpDelete("lessons/{id}")]
        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userId = User.GetUserId();
            var userRole = User.GetUserRole();

            _logger.LogInformation("User {UserId} deleting lesson: {LessonId}", userId, id);

            await _lessonService.DeleteAsync(id, userId, userRole);

            _logger.LogInformation("Lesson deleted successfully: {LessonId}", id);

            return NoContent();
        }
    }
}
