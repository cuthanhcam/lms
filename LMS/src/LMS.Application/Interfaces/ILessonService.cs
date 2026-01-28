using LMS.Application.DTOs.Lessons;

namespace LMS.Application.Interfaces
{
    public interface ILessonService
    {
        Task<LessonDto> CreateAsync(Guid courseId, CreateLessonRequest request, Guid userId, string userRole);
        Task<LessonDto> UpdateAsync(Guid id, UpdateLessonRequest request, Guid userId, string userRole);
        Task DeleteAsync(Guid id, Guid userId, string userRole);
        Task<IEnumerable<LessonDto>> GetByCourseIdAsync(Guid courseId);
        Task<LessonDto> GetByIdAsync(Guid id);
    }
}
