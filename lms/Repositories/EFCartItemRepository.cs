using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lms.Data;
using lms.Interfaces;
using lms.Models;
using Microsoft.EntityFrameworkCore;

namespace lms.Repositories
{
    public class EFCartItemRepository : ICartItemRepository
    {
        private readonly ApplicationDbContext _context;
        public EFCartItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsByStudentIdAsync(int studentId)
        {
            return await _context.CartItems
                .Include(c => c.Course)
                .ThenInclude(c => c.Category)
                .Include(c => c.Course.Images)
                .Where(c => c.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<CartItem> GetByIdAsync(int id)
        {
            return await _context.CartItems
                .Include(c => c.Course)
                .ThenInclude(c => c.Category)
                .Include(c => c.Course.Images)
                .FirstOrDefaultAsync(c => c.Id == id) ?? throw new InvalidOperationException("CartItem not found.");
        }

        public async Task AddAsync(CartItem cartItem)
        {
            _context.CartItems.Add(cartItem);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var cartItem = await _context.CartItems.FindAsync(id);
            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("CartItem not found.");
            }
        }

        public async Task<bool> ExistsAsync(int studentId, int courseId)
        {
            return await _context.CartItems
                .AnyAsync(c => c.StudentId == studentId && c.CourseId == courseId);
        }
    }
}