using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Repositories
{
    public interface IEnrollmentRepository : IGenericRepository<Enrollment>
    {
        Task<IEnumerable<Enrollment>> GetByUserIdAsync(Guid userId);
        Task<bool> IsEnrolledAsync(Guid userId, Guid courseId);
    }
}
