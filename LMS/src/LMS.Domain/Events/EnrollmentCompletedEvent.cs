namespace LMS.Domain.Events
{
    /// <summary>
    /// Domain event raised when a student completes a course
    /// 
    /// This event can trigger:
    /// - Issuing course completion certificate
    /// - Sending congratulations email
    /// - Updating student's profile/achievements
    /// - Recording completion in analytics
    /// - Suggesting related courses
    /// </summary>
    public class EnrollmentCompletedEvent : DomainEvent
    {
        /// <summary>
        /// ID of the enrollment
        /// </summary>
        public Guid EnrollmentId { get; }

        /// <summary>
        /// ID of the user who completed the course
        /// </summary>
        public Guid UserId { get; }

        /// <summary>
        /// ID of the completed course
        /// </summary>
        public Guid CourseId { get; }

        /// <summary>
        /// When the enrollment was completed
        /// </summary>
        public DateTime CompletedDate { get; }

        /// <summary>
        /// How long it took to complete (from enrollment to completion)
        /// </summary>
        public TimeSpan Duration { get; }

        public EnrollmentCompletedEvent(
            Guid enrollmentId, 
            Guid userId, 
            Guid courseId, 
            DateTime completedDate,
            TimeSpan duration)
        {
            EnrollmentId = enrollmentId;
            UserId = userId;
            CourseId = courseId;
            CompletedDate = completedDate;
            Duration = duration;
        }
    }
}
