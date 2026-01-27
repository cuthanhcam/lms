using SimpleLMS.Domain.Common;
using SimpleLMS.Domain.Enums;
using SimpleLMS.Domain.Exceptions;
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

        // Constructor for EF Core
        private Enrollment() { }

        public Enrollment(Guid userId, Guid courseId)
        {
            if (userId == Guid.Empty)
                throw new BusinessRuleViolationException("UserId cannot be empty.", nameof(userId));
            if (courseId == Guid.Empty)
                throw new BusinessRuleViolationException("CourseId cannot be empty.", nameof(courseId));

            UserId = userId;
            CourseId = courseId;
            EnrolledAt = DateTime.UtcNow;
            Status = EnrollmentStatus.Active;
            ProgressPercentage = 0;
        }

        // Update progress of the enrollment
        public void UpdateProgress(int progressPercentage)
        {
            if (progressPercentage < 0 || progressPercentage > 100)
                throw new BusinessRuleViolationException("Progress percentage must be between 0 and 100.", nameof(progressPercentage));
            if (Status != EnrollmentStatus.Active)
                throw new BusinessRuleViolationException("Cannot update progress for a non-active enrollment.", nameof(Status));

            ProgressPercentage = progressPercentage;

            if (ProgressPercentage == 100)
            {
                Complete();
            } else
            {
                MarkAsModified();
            }
        }

        // Complete the enrollment
        public void Complete()
        {
            if (Status != EnrollmentStatus.Active)
                throw new BusinessRuleViolationException("Only active enrollments can be completed.", nameof(Status));
            
            Status = EnrollmentStatus.Completed;
            ProgressPercentage = 100;
            CompletedAt = DateTime.UtcNow;
            MarkAsModified();
        }

        // Cancel the enrollment
        public void Cancel()
        {
            if (Status != EnrollmentStatus.Active)
                throw new BusinessRuleViolationException("Only active enrollments can be cancelled.", nameof(Status));
           
            Status = EnrollmentStatus.Cancelled;
            MarkAsModified();
        }   
    }
}
