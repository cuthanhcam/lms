using LMS.Domain.Entities;
using System.Linq.Expressions;

namespace LMS.Domain.Specifications
{
    /// <summary>
    /// Specification for active enrollments only
    /// 
    /// Business Rule: Enrollment status must be Active
    /// 
    /// Usage:
    /// var spec = new ActiveEnrollmentsSpecification();
    /// var activeEnrollments = await repository.FindAsync(spec);
    /// </summary>
    public class ActiveEnrollmentsSpecification : Specification<Enrollment>
    {
        public override Expression<Func<Enrollment, bool>> ToExpression()
        {
            return enrollment => enrollment.Status == EnrollmentStatus.Active;
        }
    }

    /// <summary>
    /// Specification for enrollments by specific user
    /// 
    /// Business Rule: Filter enrollments by UserId
    /// 
    /// Usage:
    /// var spec = new EnrollmentsByUserSpecification(userId);
    /// var userEnrollments = await repository.FindAsync(spec);
    /// </summary>
    public class EnrollmentsByUserSpecification : Specification<Enrollment>
    {
        private readonly Guid _userId;

        public EnrollmentsByUserSpecification(Guid userId)
        {
            _userId = userId;
        }

        public override Expression<Func<Enrollment, bool>> ToExpression()
        {
            return enrollment => enrollment.UserId == _userId;
        }
    }

    /// <summary>
    /// Specification for enrollments in specific course
    /// 
    /// Business Rule: Filter enrollments by CourseId
    /// 
    /// Usage:
    /// var spec = new EnrollmentsByCourseSpecification(courseId);
    /// var courseEnrollments = await repository.FindAsync(spec);
    /// </summary>
    public class EnrollmentsByCourseSpecification : Specification<Enrollment>
    {
        private readonly Guid _courseId;

        public EnrollmentsByCourseSpecification(Guid courseId)
        {
            _courseId = courseId;
        }

        public override Expression<Func<Enrollment, bool>> ToExpression()
        {
            return enrollment => enrollment.CourseId == _courseId;
        }
    }

    /// <summary>
    /// Specification for completed enrollments
    /// 
    /// Business Rule: Enrollment status must be Completed
    /// 
    /// Usage:
    /// var spec = new CompletedEnrollmentsSpecification();
    /// var completedEnrollments = await repository.FindAsync(spec);
    /// </summary>
    public class CompletedEnrollmentsSpecification : Specification<Enrollment>
    {
        public override Expression<Func<Enrollment, bool>> ToExpression()
        {
            return enrollment => enrollment.Status == EnrollmentStatus.Completed;
        }
    }

    /// <summary>
    /// Specification for enrollments with progress above threshold
    /// 
    /// Business Rule: Filter by minimum progress percentage
    /// 
    /// Usage:
    /// var spec = new EnrollmentsWithProgressAboveSpecification(50);
    /// var enrollmentsAbove50Percent = await repository.FindAsync(spec);
    /// </summary>
    public class EnrollmentsWithProgressAboveSpecification : Specification<Enrollment>
    {
        private readonly decimal _minimumProgress;

        public EnrollmentsWithProgressAboveSpecification(decimal minimumProgress)
        {
            _minimumProgress = minimumProgress;
        }

        public override Expression<Func<Enrollment, bool>> ToExpression()
        {
            return enrollment => enrollment.ProgressPercentage >= _minimumProgress;
        }
    }

    /// <summary>
    /// Specification for recent enrollments within specified days
    /// 
    /// Business Rule: Enrollment date must be within last N days
    /// 
    /// Usage:
    /// var spec = new RecentEnrollmentsSpecification(7); // Last 7 days
    /// var recentEnrollments = await repository.FindAsync(spec);
    /// </summary>
    public class RecentEnrollmentsSpecification : Specification<Enrollment>
    {
        private readonly int _days;

        public RecentEnrollmentsSpecification(int days)
        {
            _days = days;
        }

        public override Expression<Func<Enrollment, bool>> ToExpression()
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-_days);
            return enrollment => enrollment.EnrollAt >= cutoffDate;
        }
    }

    /// <summary>
    /// Specification for checking if user is already enrolled in a course
    /// 
    /// Business Rule: User has active enrollment in specific course
    /// Used to prevent duplicate enrollments
    /// 
    /// Usage:
    /// var spec = new UserAlreadyEnrolledSpecification(userId, courseId);
    /// var exists = await repository.AnyAsync(spec);
    /// </summary>
    public class UserAlreadyEnrolledSpecification : Specification<Enrollment>
    {
        private readonly Guid _userId;
        private readonly Guid _courseId;

        public UserAlreadyEnrolledSpecification(Guid userId, Guid courseId)
        {
            _userId = userId;
            _courseId = courseId;
        }

        public override Expression<Func<Enrollment, bool>> ToExpression()
        {
            return enrollment => enrollment.UserId == _userId 
                              && enrollment.CourseId == _courseId
                              && enrollment.Status == EnrollmentStatus.Active;
        }
    }
}
