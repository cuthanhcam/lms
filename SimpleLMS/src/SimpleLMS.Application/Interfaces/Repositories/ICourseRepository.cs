using SimpleLMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Application.Interfaces.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<IEnumerable<Course>> GetPublishedCoursesAsync();
        Task<IEnumerable<Course>> GetCoursesByInstructorAsync(Guid instructorId);
        Task<Course?> GetCourseWithLessonsAsync(Guid courseId);
        Task<Course?> GetCourseWithEnrollmentsAsync(Guid courseId);
    }
}
