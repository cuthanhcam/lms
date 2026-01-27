using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLMS.Application.DTOs.Enrollments;
using SimpleLMS.Application.Interfaces.Services;
using SimpleLMS.Domain.Entities;
using System.Security.Claims;

namespace SimpleLMS.API.Controllers
{
    /// <summary>
    /// Controller for managing student enrollments in courses and tracking their progress.
    /// </summary>
    [ApiController]
    [Route("api/enrollments")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// Retrieves a specific enrollment by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the enrollment.</param>
        /// <returns>The enrollment details.</returns>
        /// <response code="200">Returns the enrollment details.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="404">If the enrollment is not found.</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _enrollmentService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves all enrollments for the currently authenticated user.
        /// </summary>
        /// <returns>A list of the user's enrollments with progress information.</returns>
        /// <response code="200">Returns the list of user's enrollments.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpGet("my-enrollments")]
        public async Task<IActionResult> GetMyEnrollments()
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _enrollmentService.GetEnrollmentsByUserAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves all enrollments for a specific user. Only accessible by administrators.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>A list of the user's enrollments.</returns>
        /// <response code="200">Returns the list of enrollments.</response>
        /// <response code="400">If the user ID is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an administrator.</response>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var result = await _enrollmentService.GetEnrollmentsByUserAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        /// <summary>
        /// Retrieves all enrollments for a specific course. Only accessible by instructors and administrators.
        /// </summary>
        /// <param name="courseId">The unique identifier of the course.</param>
        /// <returns>A list of enrollments for the specified course.</returns>
        /// <response code="200">Returns the list of enrollments.</response>
        /// <response code="400">If the course ID is invalid.</response>
        /// <response code="401">If the user is not authenticated.</response>
        /// <response code="403">If the user is not an instructor or administrator.</response>
        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> GetByCourse(Guid courseId)
        {
            var result = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        /// <summary>
        /// Enrolls the current user in a course.
        /// </summary>
        /// <param name="dto">The enrollment details including the course ID.</param>
        /// <returns>The newly created enrollment.</returns>
        /// <response code="201">Returns the newly created enrollment.</response>
        /// <response code="400">If the enrollment is invalid or user is already enrolled.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPost]
        public async Task<IActionResult> Enroll([FromBody] CreateEnrollmentDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _enrollmentService.EnrollAsync(userId, dto);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
        }

        /// <summary>
        /// Updates the learning progress for an enrollment.
        /// </summary>
        /// <param name="enrollmentId">The unique identifier of the enrollment.</param>
        /// <param name="dto">The progress update information.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">If the progress was successfully updated.</response>
        /// <response code="400">If the update is invalid or user doesn't own the enrollment.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPut("{enrollmentId}/progress")]
        public async Task<IActionResult> UpdateProgress(Guid enrollmentId, [FromBody] UpdateProgressDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _enrollmentService.UpdateProgressAsync(enrollmentId, userId, dto);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Progress updated successfully" });
        }

        /// <summary>
        /// Marks an enrollment as completed.
        /// </summary>
        /// <param name="enrollmentId">The unique identifier of the enrollment.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">If the enrollment was successfully marked as completed.</response>
        /// <response code="400">If the completion is invalid or user doesn't own the enrollment.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPut("{enrollmentId}/complete")]
        public async Task<IActionResult> Complete(Guid enrollmentId)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _enrollmentService.CompleteAsync(enrollmentId, userId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Enrollment completed successfully" });
        }

        /// <summary>
        /// Cancels an active enrollment.
        /// </summary>
        /// <param name="enrollmentId">The unique identifier of the enrollment.</param>
        /// <returns>A success message.</returns>
        /// <response code="200">If the enrollment was successfully cancelled.</response>
        /// <response code="400">If the cancellation is invalid or user doesn't own the enrollment.</response>
        /// <response code="401">If the user is not authenticated.</response>
        [HttpPut("{enrollmentId}/cancel")]
        public async Task<IActionResult> Cancel(Guid enrollmentId)
        {
            var userId = GetCurrentUserId();
            if (userId == Guid.Empty)
                return Unauthorized(new { message = "User not authenticated" });

            var result = await _enrollmentService.CancelAsync(enrollmentId, userId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Enrollment cancelled successfully" });
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }
}
