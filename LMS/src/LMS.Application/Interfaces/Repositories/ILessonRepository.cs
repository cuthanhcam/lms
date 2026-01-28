using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {
        Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId);
        Task<Lesson?> GetByIdWithCourseAsync(Guid id);
    }
}
