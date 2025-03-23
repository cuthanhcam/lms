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
    public class EFStudentCourseRepository : IStudentCourseRepository
    {
        private readonly ApplicationDbContext _context;

        public EFStudentCourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IEnumerable<StudentCourse>> GetEnrolledCoursesByStudentIdAsync(int studentId)
        {
            return await _context.StudentCourses
                .Include(sc => sc.Course)
                .ThenInclude(c => c.Category)
                .Include(sc => sc.Course.Images)
                .Where(sc => sc.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<StudentCourse> GetByIdAsync(int id)
        {
            return await _context.StudentCourses
                .Include(sc => sc.Course)
                .ThenInclude(c => c.Category)
                .Include(sc => sc.Course.Images)
                .FirstOrDefaultAsync(sc => sc.Id == id) ?? throw new InvalidOperationException("StudentCourse not found.");
        }

        public async Task AddAsync(StudentCourse studentCourse)
        {
            _context.StudentCourses.Add(studentCourse);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int studentId, int courseId)
        {
            return await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == courseId);
        }
    }
}