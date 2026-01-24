using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Domain.Entities
{
    public class Lesson
    {
        public string Title { get; private set; }
        public string? Content { get; private set; }
        public string? VideoUrl { get; private set; }
        public int Order { get; private set; }
        public int DurationMinutes { get; private set; }
        public Guid CourseId { get; private set; }

        // Navigation properties
        public Course? Course { get; private set; }
    }
}
