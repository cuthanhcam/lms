using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Unit of Work interface for managing repositories and transactions.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        ICourseRepository Courses { get; }
        ILessonRepository Lessons { get; }
        IEnrollmentRepository Enrollments { get; }

        // Saves all changes made in the context to the database.
        Task<int> SaveChangesAsync();

        // Transaction management methods
        Task BeginTransactionAsync();

        // Commits the current transaction.
        Task CommitTransactionAsync();

        // Rolls back the current transaction.
        Task RollbackTransactionAsync();
    }
}
