namespace SimpleLMS.Application.DTOs.Courses
{
    public class CreateCourseDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
    }
}
