using SimpleLMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Application.Interfaces.Repositories
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<IEnumerable<Enrollment>> GetEnrollmentsByUserAsync(Guid userId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseAsync(Guid courseId);
        Task<Enrollment?> GetEnrollmentWithDetailsAsync(Guid enrollmentId);
        Task<bool> UserAlreadyEnrolledAsync(Guid userId, Guid courseId);
    }
}
