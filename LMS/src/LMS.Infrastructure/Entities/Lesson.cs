using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Entities
{
    public class Lesson
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid CourseId { get; set; }
        public Course? Course { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
