namespace LMS.Application.DTOs.Courses
{
    public class CreateCourseRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
