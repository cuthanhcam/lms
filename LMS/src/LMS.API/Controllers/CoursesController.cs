using LMS.API.Extensions;
using LMS.Application.DTOs.Courses;
using LMS.Application.Interfaces;
using LMS.Shared.Common;
using LMS.Shared.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers
{
    /// <summary>
    /// Controller managing Courses
    /// Includes: CRUD courses, role-based access control Admin/Instructor/Student
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    [Authorize] // All endpoints require authentication
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly ILogger<CoursesController> _logger;

        /// <summary>
        /// Constructor inject dependencies
        /// </summary>
        public CoursesController(ICourseService courseService, ILogger<CoursesController> logger)
        {
            _courseService = courseService;
            _logger = logger;
        }

        /// <summary>
        /// Get list of courses with pagination and filters
        /// </summary>
        /// <param name="parameters">Query parameters: page, pageSize, search, isPublished, minPrice, maxPrice</param>
        /// <returns>List of courses with pagination info</returns>
        /// <response code="200">Returns list of courses</response>
        [HttpGet]
        [AllowAnonymous] // Allow anonymous users to view course list
        [ProducesResponseType(typeof(PagedResult<CourseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PagedResult<CourseDto>>> GetAll([FromQuery] CourseQueryParameters parameters)
        {
            _logger.LogInformation("Getting courses list with parameters: {@Parameters}", parameters);

            var result = await _courseService.GetAllAsync(parameters);

            return Ok(result);
        }

        /// <summary>
        /// Get course details by ID
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <returns>Course details</returns>
        /// <response code="200">Returns course details</response>
        /// <response code="404">Course not found</response>
        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseDto>> GetById(Guid id)
        {
            _logger.LogInformation("Getting course by ID: {CourseId}", id);

            var course = await _courseService.GetByIdAsync(id);

            return Ok(course);
        }

        /// <summary>
        /// Get list of courses created by current user (for Instructor)
        /// </summary>
        /// <returns>List of instructor's courses</returns>
        /// <response code="200">Returns list of courses</response>
        [HttpGet("my-courses")]
        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [ProducesResponseType(typeof(IEnumerable<CourseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetMyCourses()
        {
            var userId = User.GetUserId();

            _logger.LogInformation("Getting courses created by user: {UserId}", userId);

            var courses = await _courseService.GetMyCourses(userId);

            return Ok(courses);
        }

        /// <summary>
        /// Create new course (Admin and Instructor only)
        /// </summary>
        /// <param name="request">Course information: Title, Description, Price</param>
        /// <returns>Created course</returns>
        /// <response code="201">Course created successfully</response>
        /// <response code="400">Validation failed</response>
        /// <response code="403">No permission (Admin and Instructor only)</response>
        [HttpPost]
        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CourseDto>> Create([FromBody] CreateCourseRequest request)
        {
            var userId = User.GetUserId();

            _logger.LogInformation("User {UserId} creating new course: {Title}", userId, request.Title);

            var course = await _courseService.CreateAsync(request, userId);

            _logger.LogInformation("Course created successfully: {CourseId}", course.Id);

            // Return 201 Created with Location header
            return CreatedAtAction(nameof(GetById), new { id = course.Id }, course);
        }

        /// <summary>
        /// Update course (Admin or Owner only)
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <param name="request">Update information</param>
        /// <returns>Updated course</returns>
        /// <response code="200">Update successful</response>
        /// <response code="400">Validation failed or cannot publish course without lessons</response>
        /// <response code="403">No permission (can only update own courses)</response>
        /// <response code="404">Course not found</response>
        [HttpPut("{id}")]
        [Authorize(Roles = $"{Roles.Instructor},{Roles.Admin}")]
        [ProducesResponseType(typeof(CourseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseDto>> Update(Guid id, [FromBody] UpdateCourseRequest request)
        {
            var userId = User.GetUserId();
            var userRole = User.GetUserRole();

            _logger.LogInformation("User {UserId} updating course: {CourseId}", userId, id);

            var course = await _courseService.UpdateAsync(id, request, userId, userRole);

            _logger.LogInformation("Course updated successfully: {CourseId}", course.Id);

            return Ok(course);
        }

        /// <summary>
        /// Delete course (Admin only) - Soft delete
        /// </summary>
        /// <param name="id">Course ID</param>
        /// <returns>No content</returns>
        /// <response code="204">Delete successful</response>
        /// <response code="403">No permission (Admin only)</response>
        /// <response code="404">Course not found</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = Roles.Admin)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var userRole = User.GetUserRole();

            _logger.LogInformation("Admin deleting course: {CourseId}", id);

            await _courseService.DeleteAsync(id, userRole);

            _logger.LogInformation("Course deleted successfully: {CourseId}", id);

            return NoContent();
        }
    }
}
