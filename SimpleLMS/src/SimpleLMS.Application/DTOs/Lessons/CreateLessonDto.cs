namespace SimpleLMS.Application.DTOs.Lessons
{
    public class CreateLessonDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Content { get; set; }
        public string? VideoUrl { get; set; }
        public int Order { get; set; }
        public int DurationMinutes { get; set; }
    }
}
