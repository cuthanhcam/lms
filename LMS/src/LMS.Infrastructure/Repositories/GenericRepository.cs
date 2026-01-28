using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Specifications;
using LMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace LMS.Infrastructure.Repositories
{
    /// <summary>
    /// Generic repository implementation with Specification Pattern support
    /// Provides common CRUD operations for all entities
    /// </summary>
    /// <typeparam name="T">Entity type</typeparam>
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        // ==================== QUERY METHODS ====================

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// Find entities using Specification Pattern
        /// </summary>
        public async Task<IEnumerable<T>> FindAsync(Specification<T> specification)
        {
            return await _dbSet.Where(specification.ToExpression()).ToListAsync();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// Get first entity matching specification
        /// </summary>
        public async Task<T?> FirstOrDefaultAsync(Specification<T> specification)
        {
            return await _dbSet.FirstOrDefaultAsync(specification.ToExpression());
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// Check if any entity matches specification
        /// </summary>
        public async Task<bool> AnyAsync(Specification<T> specification)
        {
            return await _dbSet.AnyAsync(specification.ToExpression());
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
                return await _dbSet.CountAsync();

            return await _dbSet.CountAsync(predicate);
        }

        /// <summary>
        /// Count entities matching specification
        /// </summary>
        public async Task<int> CountAsync(Specification<T> specification)
        {
            return await _dbSet.CountAsync(specification.ToExpression());
        }

        // ==================== COMMAND METHODS ====================

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
    }
}
