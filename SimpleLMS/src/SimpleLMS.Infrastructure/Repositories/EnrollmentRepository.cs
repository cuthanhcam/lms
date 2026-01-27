using Microsoft.EntityFrameworkCore;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Infrastructure.Persistence;

namespace SimpleLMS.Infrastructure.Repositories
{
    /// <summary>
    /// Repository for managing Enrollment entities.
    /// </summary>
    public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
    {
        public EnrollmentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByUserAsync(Guid userId)
        {
            return await _dbSet
                .Include(e => e.Course)
                    .ThenInclude(c => c!.Instructor)
                .Where(e => e.UserId == userId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(Guid courseId)
        {
            return await _dbSet
                .Include(e => e.User)
                .Where(e => e.CourseId == courseId)
                .OrderByDescending(e => e.EnrolledAt)
                .ToListAsync();
        }

        public async Task<Enrollment?> GetEnrollmentWithDetailsAsync(Guid enrollmentId)
        {
            return await _dbSet
                .Include(e => e.User)
                .Include(e => e.Course)
                    .ThenInclude(c => c!.Instructor)
                .FirstOrDefaultAsync(e => e.Id == enrollmentId);
        }

        public async Task<bool> UserAlreadyEnrolledAsync(Guid userId, Guid courseId)
        {
            return await _dbSet
                .AnyAsync(e => e.UserId == userId &&
                              e.CourseId == courseId &&
                              e.Status != Domain.Enums.EnrollmentStatus.Cancelled);
        }
    }
}
