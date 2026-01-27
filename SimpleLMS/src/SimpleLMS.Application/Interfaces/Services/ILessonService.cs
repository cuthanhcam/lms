using SimpleLMS.Application.Common;
using SimpleLMS.Application.DTOs.Lessons;

namespace SimpleLMS.Application.Interfaces.Services
{
    public interface ILessonService
    {
        Task<Result<LessonDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<LessonDto>>> GetLessonsByCourseAsync(Guid courseId);
        Task<Result<LessonDto>> CreateAsync(Guid courseId, Guid instructorId, CreateLessonDto dto);
        Task<Result<LessonDto>> UpdateAsync(Guid id, Guid instructorId, UpdateLessonDto dto);
        Task<Result> DeleteAsync(Guid id, Guid instructorId);
    }
}
