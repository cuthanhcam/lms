using LMS.Domain.Specifications;
using System.Linq.Expressions;

namespace LMS.Application.Interfaces.Repositories
{
    /// <summary>
    /// Generic repository interface for common CRUD operations
    /// Supports both Expression and Specification Pattern for queries
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        // ==================== QUERY METHODS ====================

        /// <summary>
        /// Get entity by ID
        /// </summary>
        Task<T?> GetByIdAsync(Guid id);

        /// <summary>
        /// Get all entities
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Find entities matching predicate expression
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Find entities matching specification
        /// This enables the Specification Pattern for reusable business rules
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Specification<T> specification);

        /// <summary>
        /// Get first entity matching predicate or null
        /// </summary>
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Get first entity matching specification or null
        /// </summary>
        Task<T?> FirstOrDefaultAsync(Specification<T> specification);

        /// <summary>
        /// Check if any entity matches predicate
        /// </summary>
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Check if any entity matches specification
        /// </summary>
        Task<bool> AnyAsync(Specification<T> specification);

        /// <summary>
        /// Count entities matching predicate
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

        /// <summary>
        /// Count entities matching specification
        /// </summary>
        Task<int> CountAsync(Specification<T> specification);

        // ==================== COMMAND METHODS ====================

        /// <summary>
        /// Add new entity
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Add multiple entities
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Update existing entity
        /// </summary>
        void Update(T entity);

        /// <summary>
        /// Remove entity
        /// </summary>
        void Remove(T entity);

        /// <summary>
        /// Remove multiple entities
        /// </summary>
        void RemoveRange(IEnumerable<T> entities);
    }
}
