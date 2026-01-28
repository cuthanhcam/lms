namespace LMS.Domain.Events
{
    /// <summary>
    /// Domain event raised when a course is deleted
    /// 
    /// This event can trigger:
    /// - Cancelling all active enrollments
    /// - Notifying enrolled students
    /// - Removing from search index
    /// - Archiving course data
    /// </summary>
    public class CourseDeletedEvent : DomainEvent
    {
        /// <summary>
        /// ID of the deleted course
        /// </summary>
        public Guid CourseId { get; }

        /// <summary>
        /// Title of the deleted course
        /// </summary>
        public string CourseTitle { get; }

        /// <summary>
        /// ID of the instructor who owned the course
        /// </summary>
        public Guid InstructorId { get; }

        /// <summary>
        /// Number of active enrollments at time of deletion
        /// </summary>
        public int ActiveEnrollmentCount { get; }

        public CourseDeletedEvent(Guid courseId, string courseTitle, Guid instructorId, int activeEnrollmentCount)
        {
            CourseId = courseId;
            CourseTitle = courseTitle;
            InstructorId = instructorId;
            ActiveEnrollmentCount = activeEnrollmentCount;
        }
    }
}
