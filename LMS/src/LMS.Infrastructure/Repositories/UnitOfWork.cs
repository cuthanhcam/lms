using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Common;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LMS.Infrastructure.Repositories
{
    /// <summary>
    /// Unit of Work pattern implementation with domain event dispatching
    /// 
    /// Responsibilities:
    /// 1. Coordinate multiple repository operations in a single transaction
    /// 2. Dispatch domain events after successful save
    /// 3. Provide access to all repositories
    /// 4. Manage database transactions
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private readonly IDomainEventDispatcher? _domainEventDispatcher;
        private IDbContextTransaction? _transaction;

        public IUserRepository Users { get; }
        public ICourseRepository Courses { get; }
        public ILessonRepository Lessons { get; }
        public IEnrollmentRepository Enrollments { get; }

        public UnitOfWork(
            AppDbContext context,
            IDomainEventDispatcher? domainEventDispatcher = null)
        {
            _context = context;
            _domainEventDispatcher = domainEventDispatcher;
            
            Users = new UserRepository(_context);
            Courses = new CourseRepository(_context);
            Lessons = new LessonRepository(_context);
            Enrollments = new EnrollmentRepository(_context);
        }

        /// <summary>
        /// Save all changes and dispatch domain events
        /// 
        /// Process:
        /// 1. Collect all domain events from tracked entities
        /// 2. Save changes to database
        /// 3. Dispatch domain events (only if save succeeds)
        /// 4. Clear domain events from entities
        /// </summary>
        public async Task<int> SaveChangesAsync()
        {
            // Step 1: Collect domain events from all entities before saving
            var domainEvents = _context.ChangeTracker
                .Entries<Entity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            // Step 2: Save changes to database
            var result = await _context.SaveChangesAsync();

            // Step 3: Dispatch domain events only after successful save
            if (domainEvents.Any() && _domainEventDispatcher != null)
            {
                await _domainEventDispatcher.DispatchAsync(domainEvents);
            }

            // Step 4: Clear domain events from entities
            var entities = _context.ChangeTracker
                .Entries<Entity>()
                .Where(e => e.Entity.DomainEvents.Any())
                .Select(e => e.Entity);

            foreach (var entity in entities)
            {
                entity.ClearDomainEvents();
            }

            return result;
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
