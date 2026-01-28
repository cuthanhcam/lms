using LMS.Domain.Common;
using LMS.Domain.Events;
using LMS.Domain.Exceptions;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Enrollment Entity - represents a student's enrollment in a course
    /// 
    /// Enrollment is an Aggregate Root because:
    /// - It has its own lifecycle independent of User and Course
    /// - It maintains enrollment status and progress
    /// - It can be queried independently
    /// 
    /// Business Rules:
    /// - User cannot enroll in same course twice (unless previous enrollment is cancelled)
    /// - Cannot enroll in unpublished or deleted courses
    /// - Enrollment date is immutable once created
    /// - Completed enrollments cannot be cancelled
    /// </summary>
    public class Enrollment : Entity, IAggregateRoot
    {
        // ==================== PROPERTIES ====================

        /// <summary>
        /// ID of the enrolled user
        /// </summary>
        public Guid UserId { get; private set; }

        /// <summary>
        /// Navigation property to User
        /// </summary>
        public User? User { get; private set; }

        /// <summary>
        /// ID of the enrolled course
        /// </summary>
        public Guid CourseId { get; private set; }

        /// <summary>
        /// Navigation property to Course
        /// </summary>
        public Course? Course { get; private set; }

        /// <summary>
        /// When the enrollment was created
        /// Immutable - set once at creation
        /// </summary>
        public DateTime EnrollAt { get; private set; }

        /// <summary>
        /// Current enrollment status
        /// Active = currently enrolled
        /// Completed = finished the course
        /// Cancelled = user cancelled enrollment
        /// </summary>
        public EnrollmentStatus Status { get; private set; }

        /// <summary>
        /// When the enrollment was completed (if applicable)
        /// </summary>
        public DateTime? CompletedAt { get; private set; }

        /// <summary>
        /// When the enrollment was cancelled (if applicable)
        /// </summary>
        public DateTime? CancelledAt { get; private set; }

        /// <summary>
        /// Student's progress percentage (0-100)
        /// Represents how much of the course is completed
        /// </summary>
        public decimal ProgressPercentage { get; private set; }

        // ==================== CONSTRUCTORS ====================

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private Enrollment()
        {
        }

        /// <summary>
        /// Private constructor with parameters
        /// Only factory method can create Enrollment
        /// </summary>
        private Enrollment(Guid userId, Guid courseId)
        {
            UserId = userId;
            CourseId = courseId;
            EnrollAt = DateTime.UtcNow;
            Status = EnrollmentStatus.Active;
            ProgressPercentage = 0;
            Id = Guid.NewGuid();
            
            // Raise domain event
            AddDomainEvent(new EnrollmentCreatedEvent(
                enrollmentId: Id,
                userId: userId,
                courseId: courseId,
                enrollmentDate: EnrollAt
            ));
        }

        // ==================== FACTORY METHODS ====================

        /// <summary>
        /// Factory method to create a new enrollment
        /// 
        /// Business Rule: Must provide both user and course IDs
        /// </summary>
        /// <param name="userId">ID of user enrolling</param>
        /// <param name="courseId">ID of course to enroll in</param>
        /// <returns>Valid Enrollment object</returns>
        public static Enrollment Create(Guid userId, Guid courseId)
        {
            // Validate user ID
            if (userId == Guid.Empty)
                throw new DomainException("User ID cannot be empty");

            // Validate course ID
            if (courseId == Guid.Empty)
                throw new DomainException("Course ID cannot be empty");

            return new Enrollment(userId, courseId);
        }

        // ==================== DOMAIN METHODS ====================

        /// <summary>
        /// Update student's progress in the course
        /// 
        /// Business rules:
        /// - Progress must be between 0 and 100
        /// - Cannot update progress of cancelled enrollment
        /// - Cannot update progress of completed enrollment
        /// - If progress reaches 100%, automatically complete the enrollment
        /// </summary>
        public void UpdateProgress(decimal percentage)
        {
            if (Status == EnrollmentStatus.Cancelled)
                throw new DomainException("Cannot update progress of cancelled enrollment");

            if (Status == EnrollmentStatus.Completed)
                throw new DomainException("Cannot update progress of completed enrollment");

            if (percentage < 0 || percentage > 100)
                throw new DomainException("Progress percentage must be between 0 and 100");

            ProgressPercentage = percentage;

            // Auto-complete when reaching 100%
            if (percentage == 100 && Status == EnrollmentStatus.Active)
            {
                Complete();
            }
        }

        /// <summary>
        /// Mark enrollment as completed
        /// 
        /// Business rules:
        /// - Can only complete active enrollments
        /// - Cannot complete cancelled enrollment
        /// - Sets completed timestamp
        /// </summary>
        public void Complete()
        {
            if (Status == EnrollmentStatus.Completed)
                throw new DomainException("Enrollment is already completed");

            if (Status == EnrollmentStatus.Cancelled)
                throw new DomainException("Cannot complete cancelled enrollment");

            Status = EnrollmentStatus.Completed;
            CompletedAt = DateTime.UtcNow;
            ProgressPercentage = 100; // Ensure 100% when completing
            
            // Calculate duration from enrollment to completion
            var duration = CompletedAt.Value - EnrollAt;
            
            // Raise domain event
            AddDomainEvent(new EnrollmentCompletedEvent(
                enrollmentId: Id,
                userId: UserId,
                courseId: CourseId,
                completedDate: CompletedAt.Value,
                duration: duration
            ));
        }

        /// <summary>
        /// Cancel the enrollment
        /// 
        /// Business rules:
        /// - Can only cancel active enrollments
        /// - Cannot cancel completed enrollment
        /// - Sets cancelled timestamp
        /// </summary>
        public void Cancel()
        {
            if (Status == EnrollmentStatus.Cancelled)
                throw new DomainException("Enrollment is already cancelled");

            if (Status == EnrollmentStatus.Completed)
                throw new DomainException("Cannot cancel completed enrollment");

            Status = EnrollmentStatus.Cancelled;
            CancelledAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Check if enrollment is active (not completed or cancelled)
        /// </summary>
        public bool IsActive()
        {
            return Status == EnrollmentStatus.Active;
        }

        /// <summary>
        /// Check if this enrollment belongs to a specific user
        /// </summary>
        public bool BelongsToUser(Guid userId)
        {
            return UserId == userId;
        }

        /// <summary>
        /// Check if this enrollment is for a specific course
        /// </summary>
        public bool IsForCourse(Guid courseId)
        {
            return CourseId == courseId;
        }
    }

    /// <summary>
    /// Enrollment status enumeration
    /// Represents the current state of an enrollment
    /// </summary>
    public enum EnrollmentStatus
    {
        /// <summary>
        /// Student is currently enrolled and can access the course
        /// </summary>
        Active = 0,

        /// <summary>
        /// Student has completed the course
        /// </summary>
        Completed = 1,

        /// <summary>
        /// Enrollment was cancelled by student or admin
        /// </summary>
        Cancelled = 2
    }
}

