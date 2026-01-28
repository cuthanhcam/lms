namespace LMS.Domain.Events
{
    /// <summary>
    /// Domain event raised when a user enrolls in a course
    /// 
    /// This event can trigger:
    /// - Sending welcome email with course details
    /// - Creating initial progress tracking
    /// - Updating course enrollment count
    /// - Notifying instructor of new student
    /// </summary>
    public class EnrollmentCreatedEvent : DomainEvent
    {
        /// <summary>
        /// ID of the enrollment
        /// </summary>
        public Guid EnrollmentId { get; }

        /// <summary>
        /// ID of the user who enrolled
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// ID of the course enrolled in
        /// </summary>
        public Guid CourseId { get; }

        /// <summary>
        /// When the enrollment occurred
        /// </summary>
        public DateTime EnrollmentDate { get; }

        public EnrollmentCreatedEvent(Guid enrollmentId, Guid userId, Guid courseId, DateTime enrollmentDate)
        {
            EnrollmentId = enrollmentId;
            UserId = userId;
            CourseId = courseId;
            EnrollmentDate = enrollmentDate;
        }
    }
}
