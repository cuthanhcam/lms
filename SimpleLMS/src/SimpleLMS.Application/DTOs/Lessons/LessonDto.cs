namespace SimpleLMS.Application.DTOs.Lessons
{
    public class LessonDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
        public int DurationMinutes { get; set; }
        public Guid CourseId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
