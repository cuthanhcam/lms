using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLMS.Application.DTOs.Courses;
using SimpleLMS.Application.Interfaces.Services;
using SimpleLMS.Domain.Entities;
using System.Security.Claims;

namespace SimpleLMS.API.Controllers
{
    /// <summary>
    /// Controller for managing courses and course-related operations.
    /// </summary>
    [ApiController]
    [Route("api/courses")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        private Guid GetCurrentUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.Parse(userId!);
        }

        /// <summary>
        /// Retrieves all published courses available for enrollment.
        /// </summary>
        /// <returns>A list of published courses with lesson counts and durations.</returns>
        /// <response code="200">Returns the list of published courses.</response>
        [HttpGet("published")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetPublishedCourses()
        {
            var result = await _courseService.GetPublishedCoursesAsync();
            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves detailed information about a specific course, including all its lessons.
        /// </summary>
        /// <param name="id">The unique identifier of the course.</param>
        /// <returns>Detailed course information with lessons.</returns>
        /// <response code="200">Returns the course details.</response>
        /// <response code="404">If the course is not found.</response>
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDetailDto>> GetById(Guid id)
        {
            var result = await _courseService.GetDetailByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves all courses created by a specific instructor.
        /// </summary>
        /// <param name="instructorId">The unique identifier of the instructor.</param>
        /// <returns>A list of courses created by the instructor.</returns>
        /// <response code="200">Returns the list of courses.</response>
        [HttpGet("instructor/{instructorId}")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetByInstructor(Guid instructorId)
        {
            var result = await _courseService.GetCoursesByInstructorAsync(instructorId);
            return Ok(result.Data);
        }

        /// <summary>
        /// Creates a new course. Only accessible by instructors and administrators.
        /// </summary>
        /// <param name="createCourseDto">The course creation details.</param>
        /// <returns>The newly created course.</returns>
        /// <response code="201">Returns the newly created course.</response>
        /// <response code="400">If the course data is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an instructor or administrator.</response>
        [HttpPost]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseDto createCourseDto)
        {
            var instructorId = GetCurrentUserId();
            var result = await _courseService.CreateAsync(instructorId, createCourseDto);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        /// <summary>
        /// Updates an existing course. Only the course creator or administrators can update.
        /// </summary>
        /// <param name="id">The unique identifier of the course.</param>
        /// <param name="updateCourseDto">The updated course information.</param>
        /// <returns>The updated course details.</returns>
        /// <response code="200">Returns the updated course.</response>
        /// <response code="400">If the update data is invalid or user doesn't have permission.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an instructor or administrator.</response>
        [HttpPut("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult<CourseDto>> Update(Guid id, [FromBody] UpdateCourseDto updateCourseDto)
        {
            var instructorId = GetCurrentUserId();
            var result = await _courseService.UpdateAsync(id, instructorId, updateCourseDto);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        /// <summary>
        /// Soft deletes a course. Only the course creator or administrators can delete.
        /// </summary>
        /// <param name="id">The unique identifier of the course to delete.</param>
        /// <returns>No content if successful.</returns>
        /// <response code="204">If the course was successfully deleted.</response>
        /// <response code="400">If the deletion failed or user doesn't have permission.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an instructor or administrator.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var instructorId = GetCurrentUserId();
            var result = await _courseService.DeleteAsync(id, instructorId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return NoContent();
        }

        /// <summary>
        /// Publishes a course, making it available for student enrollment.
        /// </summary>
        /// <param name="id">The unique identifier of the course to publish.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">If the course was successfully published.</response>
        /// <response code="400">If the course cannot be published or user doesn't have permission.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an instructor or administrator.</response>
        [HttpPost("{id}/publish")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult> Publish(Guid id)
        {
            var instructorId = GetCurrentUserId();
            var result = await _courseService.PublishAsync(id, instructorId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Course published successfully" });
        }

        /// <summary>
        /// Unpublishes a course, making it unavailable for new student enrollments.
        /// </summary>
        /// <param name="id">The unique identifier of the course to unpublish.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">If the course was successfully unpublished.</response>
        /// <response code="400">If the course cannot be unpublished or user doesn't have permission.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an instructor or administrator.</response>
        [HttpPost("{id}/unpublish")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<ActionResult> Unpublish(Guid id)
        {
            var instructorId = GetCurrentUserId();
            var result = await _courseService.UnpublishAsync(id, instructorId);

            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Course unpublished successfully" });
        }
    }
}
