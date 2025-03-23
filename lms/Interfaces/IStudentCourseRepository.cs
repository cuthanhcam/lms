using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using lms.Models;

namespace lms.Interfaces
{
    public interface IStudentCourseRepository
    {
        Task<IEnumerable<StudentCourse>> GetEnrolledCoursesByStudentIdAsync(int studentId);
        Task<StudentCourse> GetByIdAsync(int id);
        Task AddAsync(StudentCourse studentCourse);
        Task<bool> ExistsAsync(int studentId, int courseId);
    }
}