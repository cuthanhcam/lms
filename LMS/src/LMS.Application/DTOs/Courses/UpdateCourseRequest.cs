namespace LMS.Application.DTOs.Courses
{
    public class UpdateCourseRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool IsPublished { get; set; }
    }
}
