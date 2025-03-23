using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lms.Models;

namespace lms.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Course> GetByIdAsync(int id);
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task DeleteAsync(int id);
    }
}