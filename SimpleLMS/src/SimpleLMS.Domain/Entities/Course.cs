using SimpleLMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace SimpleLMS.Domain.Entities
{
    public class Course : BaseEntity
    {
        public string Title { set; private set; }
        public string? Description { set; private set; }
        public decimal Price { get; private set; }
        public Guid InstructorId { get; private set; }
        public bool IsPublished { get; private set; }

        // Navigation properties
        public User? Instructor { get; private set; }

        private readonly List<Lesson> _lessons = new();
        public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

        private readonly List<Enrollment> _enrollments = new();
        public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    }
}
