namespace SimpleLMS.Application.DTOs.Courses
{
    public class CourseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public Guid InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public int TotalLessons { get; set; }
        public int TotalDurationMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
