using LMS.Application.DTOs.Enrollments;

namespace LMS.Application.Interfaces
{
    public interface IEnrollmentService
    {
        Task<EnrollmentDto> EnrollAsync(Guid courseId, Guid userId);
        Task<IEnumerable<EnrollmentDto>> GetMyCoursesAsync(Guid userId);
    }
}
