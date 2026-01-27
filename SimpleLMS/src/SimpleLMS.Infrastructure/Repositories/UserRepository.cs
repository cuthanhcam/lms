using Microsoft.EntityFrameworkCore;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Infrastructure.Persistence;

namespace SimpleLMS.Infrastructure.Repositories
{
    /// <summary>
    /// User repository implementation.
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email.ToLower());
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _dbSet
                .AnyAsync(u => u.Username == username);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _dbSet
                .AnyAsync(u => u.Email == email.ToLower());
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _dbSet
                .Where(u => u.Role.ToString() == role)
                .ToListAsync();
        }
    }
}
