using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Infrastructure.Entities
{
    public class Course
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Guid CreatedBy { get; set; } // FK to User
        public User? CreatedByUser { get; set; } // Navigation
        public bool IsPublished { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
