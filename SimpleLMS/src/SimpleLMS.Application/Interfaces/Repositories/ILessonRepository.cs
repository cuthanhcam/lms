using SimpleLMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Application.Interfaces.Repositories
{
    public interface ILessonRepository : IRepository<Lesson>
    {
        Task<IEnumerable<Lesson>> GetLessonsByCourseAsync(Guid courseId);
        Task<Lesson?> GetLessonWithCourseAsync(Guid lessonId);
    }
}
