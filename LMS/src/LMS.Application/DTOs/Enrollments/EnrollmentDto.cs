namespace LMS.Application.DTOs.Enrollments
{
    public class EnrollmentDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
        public string? CourseDescription { get; set; }
        public decimal CoursePrice { get; set; }
        public DateTime EnrollAt { get; set; }
        public int TotalLessons { get; set; }
    }
}
