using LMS.API.Extensions;
using LMS.Application.DTOs.Enrollments;
using LMS.Application.Interfaces;
using LMS.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    /// <summary>
    /// Controller managing Enrollments (course registration)
    /// Includes: Enroll course, view enrolled courses
    /// </summary>
    [ApiController]
    [Route("api")]
    [Produces("application/json")]
    [Authorize]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<EnrollmentsController> _logger;

        /// <summary>
        /// Constructor inject dependencies
        /// </summary>
        public EnrollmentsController(IEnrollmentService enrollmentService, ILogger<EnrollmentsController> logger)
        {
            _enrollmentService = enrollmentService;
            _logger = logger;
        }

        /// <summary>
        /// Enroll in a course (Student only)
        /// </summary>
        /// <param name="courseId">Course ID to enroll</param>
        /// <returns>Enrollment information</returns>
        /// <response code="201">Enrollment successful</response>
        /// <response code="400">
        /// - Course not published yet
        /// - Already enrolled in this course
        /// </response>
        /// <response code="404">Course not found</response>
        [HttpPost("courses/{courseId}/enroll")]
        [Authorize(Roles = Roles.Student)]
        [ProducesResponseType(typeof(EnrollmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EnrollmentDto>> Enroll(Guid courseId)
        {
            var userId = User.GetUserId();

            _logger.LogInformation("User {UserId} enrolling in course: {CourseId}", userId, courseId);

            var enrollment = await _enrollmentService.EnrollAsync(courseId, userId);

            _logger.LogInformation("User enrolled successfully: {EnrollmentId}", enrollment.Id);

            return CreatedAtAction(nameof(GetMyCourses), null, enrollment);
        }

        /// <summary>
        /// Get list of courses that user has enrolled in
        /// </summary>
        /// <returns>List of enrollments</returns>
        /// <response code="200">Returns list of enrolled courses</response>
        [HttpGet("my-enrollments")]
        [Authorize(Roles = Roles.Student)]
        [ProducesResponseType(typeof(IEnumerable<EnrollmentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EnrollmentDto>>> GetMyCourses()
        {
            var userId = User.GetUserId();

            _logger.LogInformation("Getting enrollments for user: {UserId}", userId);

            var enrollments = await _enrollmentService.GetMyCoursesAsync(userId);

            return Ok(enrollments);
        }
    }
}
