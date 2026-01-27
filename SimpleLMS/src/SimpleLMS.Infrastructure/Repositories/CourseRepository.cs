using Microsoft.EntityFrameworkCore;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Infrastructure.Persistence;

namespace SimpleLMS.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing Course entities.
    /// </summary>
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext context) : base(context)
        {
        }

        public override async Task<Course?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public override async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _dbSet
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
        {
            return await _dbSet
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Where(c => c.IsPublished)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Course>> GetCoursesByInstructorAsync(Guid instructorId)
        {
            return await _dbSet
                .Include(c => c.Instructor)
                .Include(c => c.Lessons)
                .Where(c => c.InstructorId == instructorId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseWithLessonsAsync(Guid courseId)
        {
            return await _dbSet
                .Include(c => c.Instructor)
                .Include(c => c.Lessons.OrderBy(l => l.Order))
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }

        public async Task<Course?> GetCourseWithEnrollmentsAsync(Guid courseId)
        {
            return await _dbSet
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(c => c.Id == courseId);
        }
    }
}
