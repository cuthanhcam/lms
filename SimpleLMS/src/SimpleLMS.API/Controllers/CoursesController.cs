using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLMS.Application.DTOs.Courses;
using SimpleLMS.Application.Interfaces.Services;
using SimpleLMS.Domain.Entities;
using System.Security.Claims;

namespace SimpleLMS.API.Controllers
{
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

        [HttpGet("published")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetPublishedCourses()
        {
            var result = await _courseService.GetPublishedCoursesAsync();
            return Ok(result.Data);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDetailDto>> GetById(Guid id)
        {
            var result = await _courseService.GetDetailByIdAsync(id);

            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpGet("instructor/{instructorId}")]
        public async Task<ActionResult<IEnumerable<CourseDto>>> GetByInstructor(Guid instructorId)
        {
            var result = await _courseService.GetCoursesByInstructorAsync(instructorId);
            return Ok(result.Data);
        }

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
