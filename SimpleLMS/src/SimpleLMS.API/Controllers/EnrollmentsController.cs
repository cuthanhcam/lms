using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SimpleLMS.Application.DTOs.Enrollments;
using SimpleLMS.Application.Interfaces.Services;
using SimpleLMS.Domain.Entities;
using System.Security.Claims;

namespace SimpleLMS.API.Controllers
{
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _enrollmentService.GetByIdAsync(id);
            if (!result.IsSuccess)
                return NotFound(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

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

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var result = await _enrollmentService.GetEnrollmentsByUserAsync(userId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

        [HttpGet("course/{courseId}")]
        [Authorize(Roles = "Instructor,Admin")]
        public async Task<IActionResult> GetByCourse(Guid courseId)
        {
            var result = await _enrollmentService.GetEnrollmentsByCourseAsync(courseId);
            if (!result.IsSuccess)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(result.Data);
        }

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
