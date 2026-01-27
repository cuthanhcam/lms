using SimpleLMS.Application.Common;
using SimpleLMS.Application.DTOs.Enrollments;

namespace SimpleLMS.Application.Interfaces.Services
{
    public interface IEnrollmentService
    {
        Task<Result<EnrollmentDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<EnrollmentDto>>> GetEnrollmentsByUserAsync(Guid userId);
        Task<Result<IEnumerable<EnrollmentDto>>> GetEnrollmentsByCourseAsync(Guid courseId);
        Task<Result<EnrollmentDto>> EnrollAsync(Guid userId, CreateEnrollmentDto dto);
        Task<Result> UpdateProgressAsync(Guid enrollmentId, Guid userId, UpdateProgressDto dto);
        Task<Result> CompleteAsync(Guid enrollmentId, Guid userId);
        Task<Result> CancelAsync(Guid enrollmentId, Guid userId);
    }
}
