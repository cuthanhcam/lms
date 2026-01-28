using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories
{
    public class EnrollmentRepository : GenericRepository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Enrollment>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(e => e.Course!)
                    .ThenInclude(c => c.Lessons)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EnrollAt)
                .ToListAsync();
        }

        public async Task<bool> IsEnrolledAsync(Guid userId, Guid courseId)
        {
            return await _dbSet.AnyAsync(e => e.UserId == userId && e.CourseId == courseId);
        }
    }
}
