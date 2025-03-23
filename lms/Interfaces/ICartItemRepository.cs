using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lms.Models;

namespace lms.Interfaces
{
    public interface ICartItemRepository
    {
        Task<IEnumerable<CartItem>> GetCartItemsByStudentIdAsync(int studentId);
        Task<CartItem> GetByIdAsync(int id);
        Task AddAsync(CartItem cartItem);
        Task DeleteAsync(int id);
        Task<bool> ExistsAsync(int studentId, int courseId);
    }
}