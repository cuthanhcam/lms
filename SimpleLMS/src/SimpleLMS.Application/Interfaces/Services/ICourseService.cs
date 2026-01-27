using SimpleLMS.Application.Common;
using SimpleLMS.Application.DTOs.Courses;

namespace SimpleLMS.Application.Interfaces.Services
{
    public interface ICourseService
    {
        Task<Result<CourseDto>> GetByIdAsync(Guid id);
        Task<Result<CourseDetailDto>> GetDetailByIdAsync(Guid id);
        Task<Result<IEnumerable<CourseDto>>> GetAllAsync();
        Task<Result<IEnumerable<CourseDto>>> GetPublishedCoursesAsync();
        Task<Result<IEnumerable<CourseDto>>> GetCoursesByInstructorAsync(Guid instructorId);
        Task<Result<CourseDto>> CreateAsync(Guid instructorId, CreateCourseDto dto);
        Task<Result<CourseDto>> UpdateAsync(Guid id, Guid instructorId, UpdateCourseDto dto);
        Task<Result> DeleteAsync(Guid id, Guid instructorId);
        Task<Result> PublishAsync(Guid id, Guid instructorId);
        Task<Result> UnpublishAsync(Guid id, Guid instructorId);
    }
}
