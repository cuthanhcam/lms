using SimpleLMS.Application.DTOs.Lessons;

namespace SimpleLMS.Application.DTOs.Courses
{
    public class CourseDetailDto : CourseDto
    {
        public List<LessonDto> Lessons { get; set; } = new();
    }
}
