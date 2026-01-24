using SimpleLMS.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FullName { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        private readonly List<Course> _createdCourses = new();
        public IReadOnlyCollection<Course> CreatedCourses => _createdCourses.AsReadOnly();

        private readonly List<Enrollment> _enrollments = new();
        public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

    }
}
