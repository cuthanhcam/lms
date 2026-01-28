using LMS.Domain.Entities;
using System.Linq.Expressions;

namespace LMS.Domain.Specifications
{
    /// <summary>
    /// Specification for published courses only
    /// 
    /// Business Rule: A course is published if IsPublished = true
    /// 
    /// Usage:
    /// var spec = new PublishedCoursesSpecification();
    /// var publishedCourses = await repository.FindAsync(spec);
    /// </summary>
    public class PublishedCoursesSpecification : Specification<Course>
    {
        public override Expression<Func<Course, bool>> ToExpression()
        {
            return course => course.IsPublished;
        }
    }

    /// <summary>
    /// Specification for courses by specific instructor
    /// 
    /// Business Rule: Filter courses by CreatedBy (instructor ID)
    /// 
    /// Usage:
    /// var spec = new CoursesByInstructorSpecification(instructorId);
    /// var instructorCourses = await repository.FindAsync(spec);
    /// </summary>
    public class CoursesByInstructorSpecification : Specification<Course>
    {
        private readonly Guid _instructorId;

        public CoursesByInstructorSpecification(Guid instructorId)
        {
            _instructorId = instructorId;
        }

        public override Expression<Func<Course, bool>> ToExpression()
        {
            return course => course.CreatedBy == _instructorId;
        }
    }

    /// <summary>
    /// Specification for courses within a price range
    /// 
    /// Business Rule: Filter courses where price is between min and max
    /// 
    /// Usage:
    /// var spec = new CoursesInPriceRangeSpecification(10, 100);
    /// var affordableCourses = await repository.FindAsync(spec);
    /// </summary>
    public class CoursesInPriceRangeSpecification : Specification<Course>
    {
        private readonly decimal _minPrice;
        private readonly decimal _maxPrice;

        public CoursesInPriceRangeSpecification(decimal minPrice, decimal maxPrice)
        {
            _minPrice = minPrice;
            _maxPrice = maxPrice;
        }

        public override Expression<Func<Course, bool>> ToExpression()
        {
            return course => course.Price.Amount >= _minPrice 
                          && course.Price.Amount <= _maxPrice;
        }
    }

    /// <summary>
    /// Specification for courses that are not deleted
    /// 
    /// Business Rule: Filter out soft-deleted courses
    /// 
    /// Usage:
    /// var spec = new ActiveCoursesSpecification();
    /// var activeCourses = await repository.FindAsync(spec);
    /// </summary>
    public class ActiveCoursesSpecification : Specification<Course>
    {
        public override Expression<Func<Course, bool>> ToExpression()
        {
            return course => !course.IsDeleted;
        }
    }

    /// <summary>
    /// Specification for courses with lessons
    /// 
    /// Business Rule: Only show courses that have at least one lesson
    /// 
    /// Usage:
    /// var spec = new CoursesWithLessonsSpecification();
    /// var coursesWithContent = await repository.FindAsync(spec);
    /// </summary>
    public class CoursesWithLessonsSpecification : Specification<Course>
    {
        public override Expression<Func<Course, bool>> ToExpression()
        {
            return course => course.Lessons.Any();
        }
    }

    /// <summary>
    /// Specification for courses that can be enrolled in
    /// 
    /// Business Rule: Course must be published, not deleted, and have lessons
    /// This is a composite specification combining multiple rules
    /// 
    /// Usage:
    /// var spec = new EnrollableCoursesSpecification();
    /// var enrollableCourses = await repository.FindAsync(spec);
    /// </summary>
    public class EnrollableCoursesSpecification : Specification<Course>
    {
        public override Expression<Func<Course, bool>> ToExpression()
        {
            return course => course.IsPublished 
                          && !course.IsDeleted 
                          && course.Lessons.Any();
        }
    }

    /// <summary>
    /// Specification for searching courses by title or description
    /// 
    /// Business Rule: Search in both Title and Description fields
    /// Case-insensitive search
    /// 
    /// Usage:
    /// var spec = new CourseSearchSpecification("programming");
    /// var matchingCourses = await repository.FindAsync(spec);
    /// </summary>
    public class CourseSearchSpecification : Specification<Course>
    {
        private readonly string _searchTerm;

        public CourseSearchSpecification(string searchTerm)
        {
            _searchTerm = searchTerm.ToLower();
        }

        public override Expression<Func<Course, bool>> ToExpression()
        {
            return course => course.Title.ToLower().Contains(_searchTerm)
                          || (course.Description != null && course.Description.ToLower().Contains(_searchTerm));
        }
    }
}
