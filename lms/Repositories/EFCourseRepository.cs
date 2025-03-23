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
    public class EFCourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;
        public EFCourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Images)
                .ToListAsync();
        }

        public async Task<Course> GetByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Category)
                .Include(c => c.Teacher)
                .Include(c => c.Images)
                .FirstOrDefaultAsync(c => c.Id == id) ?? throw new InvalidOperationException("Course not found.");
        }

        public async Task AddAsync(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Course course)
        {
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new InvalidOperationException("Course not found.");
            }
        }
    }
}