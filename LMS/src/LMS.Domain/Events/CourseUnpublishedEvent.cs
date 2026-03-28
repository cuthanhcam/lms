namespace LMS.Domain.Events
{
    /// <summary>
    /// Domain event raised when a course is unpublished.
    /// </summary>
    public class CourseUnpublishedEvent : DomainEvent
    {
        public Guid CourseId { get; }
        public string CourseTitle { get; }
        public Guid InstructorId { get; }

        public CourseUnpublishedEvent(Guid courseId, string courseTitle, Guid instructorId)
        {
            CourseId = courseId;
            CourseTitle = courseTitle;
            InstructorId = instructorId;
        }
    }
}
