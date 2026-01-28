namespace LMS.Domain.Events
{
    /// <summary>
    /// Domain event raised when a course is published
    /// 
    /// This event can trigger:
    /// - Sending notifications to followers/subscribers
    /// - Updating course catalog/search index
    /// - Logging for analytics
    /// - Updating statistics
    /// </summary>
    public class CoursePublishedEvent : DomainEvent
    {
        /// <summary>
        /// ID of the course that was published
        /// </summary>
        public Guid CourseId { get; }

        /// <summary>
        /// Title of the published course
        /// </summary>
        public string CourseTitle { get; }

        /// <summary>
        /// ID of the instructor who published the course
        /// </summary>
        public Guid InstructorId { get; }

        /// <summary>
        /// Number of lessons in the course
        /// </summary>
        public int LessonCount { get; }

        public CoursePublishedEvent(Guid courseId, string courseTitle, Guid instructorId, int lessonCount)
        {
            CourseId = courseId;
            CourseTitle = courseTitle;
            InstructorId = instructorId;
            LessonCount = lessonCount;
        }
    }
}
