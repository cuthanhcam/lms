using LMS.Domain.Entities;
using System.Reflection;

namespace LMS.Application.UnitTests.Builders
{
    /// <summary>
    /// Builder pattern to create Lesson entity for testing
    /// </summary>
    public class LessonBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _courseId = Guid.NewGuid();
        private Course? _course = null;
        private string _title = "Test Lesson";
        private string _content = "Test Content";
        private int _order = 1;
        private bool _isDeleted = false;

        public LessonBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public LessonBuilder WithCourseId(Guid courseId)
        {
            _courseId = courseId;
            return this;
        }

        public LessonBuilder WithCourse(Course course)
        {
            _course = course;
            _courseId = course.Id;
            return this;
        }

        public LessonBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public LessonBuilder WithContent(string content)
        {
            _content = content;
            return this;
        }

        public LessonBuilder WithOrder(int order)
        {
            _order = order;
            return this;
        }

        public LessonBuilder AsDeleted()
        {
            _isDeleted = true;
            return this;
        }

        public Lesson Build()
        {
            // Use factory method to create lesson
            var lesson = Lesson.Create(_title, _content, _order);
            
            // Use reflection to set Id (for testing only)
            var idProperty = typeof(Lesson).BaseType!.GetProperty("Id");
            idProperty!.SetValue(lesson, _id);
            
            // Use reflection to set CourseId
            var courseIdField = typeof(Lesson).GetProperty("CourseId", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            courseIdField!.SetValue(lesson, _courseId);
            
            // Set Course navigation property if provided
            if (_course != null)
            {
                var courseProp = typeof(Lesson).GetProperty("Course", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                courseProp!.SetValue(lesson, _course);
            }
            
            // Use reflection to set IsDeleted if needed
            if (_isDeleted)
            {
                var isDeletedField = typeof(Lesson).GetProperty("IsDeleted", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                isDeletedField!.SetValue(lesson, _isDeleted);
            }
            
            return lesson;
        }
    }
}
