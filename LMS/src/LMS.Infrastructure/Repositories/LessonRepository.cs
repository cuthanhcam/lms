using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories
{
    public class LessonRepository : GenericRepository<Lesson>, ILessonRepository
    {
        public LessonRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Lesson>> GetByCourseIdAsync(Guid courseId)
        {
            return await _dbSet
                .Where(l => l.CourseId == courseId)
                .OrderBy(l => l.Order)
                .ToListAsync();
        }

        public async Task<Lesson?> GetByIdWithCourseAsync(Guid id)
        {
            return await _dbSet
                .Include(l => l.Course)
                .FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}
