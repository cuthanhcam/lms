using LMS.Domain.Entities;
using LMS.Domain.ValueObjects;
using System.Reflection;

namespace LMS.Application.UnitTests.Builders
{
    /// <summary>
    /// Builder pattern to create Course entity for testing
    /// </summary>
    public class CourseBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _title = "Test Course";
        private string? _description = "Test Description";
        private decimal _price = 100.00m;
        private Guid _createdBy = Guid.NewGuid();
        private User? _createdByUser = null;
        private bool _isPublished = false;
        private bool _isDeleted = false;
        private DateTime _createdAt = DateTime.UtcNow;
        private List<Lesson> _lessons = new();

        public CourseBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public CourseBuilder WithTitle(string title)
        {
            _title = title;
            return this;
        }

        public CourseBuilder WithDescription(string? description)
        {
            _description = description;
            return this;
        }

        public CourseBuilder WithPrice(decimal price)
        {
            _price = price;
            return this;
        }

        public CourseBuilder WithCreatedBy(Guid createdBy)
        {
            _createdBy = createdBy;
            return this;
        }

        public CourseBuilder WithCreatedByUser(User user)
        {
            _createdByUser = user;
            _createdBy = user.Id;
            return this;
        }

        public CourseBuilder AsPublished()
        {
            _isPublished = true;
            return this;
        }

        public CourseBuilder AsDeleted()
        {
            _isDeleted = true;
            return this;
        }

        public CourseBuilder WithLessons(List<Lesson> lessons)
        {
            _lessons = lessons;
            return this;
        }

        public CourseBuilder WithLesson(Lesson lesson)
        {
            _lessons.Add(lesson);
            return this;
        }

        public Course Build()
        {
            // Create Money value object
            var price = Money.Create(_price, "USD");
            
            // Use factory method to create course
            var course = Course.Create(_title, _description, price, _createdBy);
            
            // Use reflection to set Id (for testing only)
            var idProperty = typeof(Course).BaseType!.GetProperty("Id");
            idProperty!.SetValue(course, _id);
            
            // Use reflection to set CreatedAt
            var createdAtField = typeof(Course).GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            createdAtField!.SetValue(course, _createdAt);
            
            // Use domain methods to publish/delete if needed
            if (_isPublished)
            {
                // Add at least one lesson before publishing (business rule)
                if (!_lessons.Any())
                {
                    var dummyLesson = Lesson.Create("Dummy", "Content", 1);
                    var lessonsField = typeof(Course).GetField("_lessons", BindingFlags.NonPublic | BindingFlags.Instance);
                    var lessonsList = lessonsField!.GetValue(course) as System.Collections.IList;
                    lessonsList!.Add(dummyLesson);
                }
                course.Publish();
            }
            
            if (_isDeleted)
            {
                var isDeletedField = typeof(Course).GetProperty("IsDeleted", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                isDeletedField!.SetValue(course, true);
            }
            
            // Add lessons using domain method
            foreach (var lesson in _lessons)
            {
                course.AddLesson(lesson);
            }
            
            // Set CreatedByUser navigation property if provided
            if (_createdByUser != null)
            {
                var createdByUserProp = typeof(Course).GetProperty("CreatedByUser", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                createdByUserProp!.SetValue(course, _createdByUser);
            }
            
            return course;
        }
    }
}
