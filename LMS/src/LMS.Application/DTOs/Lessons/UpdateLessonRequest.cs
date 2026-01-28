namespace LMS.Application.DTOs.Lessons
{
    public class UpdateLessonRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int Order { get; set; }
    }
}
