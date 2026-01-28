using LMS.Application.DTOs.Courses;
using LMS.Shared.Common;

namespace LMS.Application.Interfaces
{
    public interface ICourseService
    {
        Task<CourseDto> CreateAsync(CreateCourseRequest request, Guid userId);
        Task<CourseDto> UpdateAsync(Guid id, UpdateCourseRequest request, Guid userId, string userRole);
        Task DeleteAsync(Guid id, string userRole);
        Task<CourseDto> GetByIdAsync(Guid id);
        Task<PagedResult<CourseDto>> GetAllAsync(CourseQueryParameters parameters);
        Task<IEnumerable<CourseDto>> GetMyCourses(Guid userId);
    }
}
