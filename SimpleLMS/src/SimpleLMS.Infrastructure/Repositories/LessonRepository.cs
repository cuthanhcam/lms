using Microsoft.EntityFrameworkCore;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Infrastructure.Persistence;

namespace SimpleLMS.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing Lesson entities.
    /// </summary>
    public class LessonRepository : Repository<Lesson>, ILessonRepository
    {
        public LessonRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Lesson>> GetLessonsByCourseAsync(Guid courseId)
        {
            return await _dbSet
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Order)
                .ToListAsync();
        }

        public async Task<Lesson?> GetLessonWithCourseAsync(Guid lessonId)
        {
            return await _dbSet
                .Include(l => l.Course)
                    .ThenInclude(c => c!.Instructor)
                .FirstOrDefaultAsync(l => l.Id == lessonId);
        }
    }
}
