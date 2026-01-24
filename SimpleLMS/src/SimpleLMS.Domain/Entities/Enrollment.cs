using SimpleLMS.Domain.Common;
using SimpleLMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Domain.Entities
{
    public class Enrollment : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid CourseId { get; private set; }
        public DateTime EnrolledAt { get; private set; }
        public EnrollmentStatus Status { get; private set; }
        public DateTime? CompletedAt { get; private set; }
        public int ProgressPercentage { get; private set; }

        // Navigation properties
        public User? User { get; private set; }
        public Course? Course { get; private set; }
    }
}
