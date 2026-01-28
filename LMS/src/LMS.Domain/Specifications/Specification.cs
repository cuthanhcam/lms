using System.Linq.Expressions;

namespace LMS.Domain.Specifications
{
    /// <summary>
    /// Base class for implementing the Specification Pattern
    /// 
    /// The Specification Pattern encapsulates business rules for querying.
    /// Benefits:
    /// 1. Reusable query logic
    /// 2. Testable business rules
    /// 3. Composable specifications (AND, OR, NOT)
    /// 4. Domain language in queries
    /// 
    /// Example usage:
    /// var spec = new PublishedCoursesSpecification()
    ///     .And(new CoursesByInstructorSpecification(instructorId));
    /// var courses = await repository.FindAsync(spec);
    /// </summary>
    /// <typeparam name="T">Entity type this specification applies to</typeparam>
    public abstract class Specification<T>
    {
        /// <summary>
        /// Expression that defines the specification criteria
        /// This is the core of the specification - the "where" clause
        /// </summary>
        public abstract Expression<Func<T, bool>> ToExpression();

        /// <summary>
        /// Check if an entity satisfies this specification
        /// Useful for in-memory validation
        /// </summary>
        public bool IsSatisfiedBy(T entity)
        {
            var predicate = ToExpression().Compile();
            return predicate(entity);
        }

        /// <summary>
        /// Combine this specification with another using AND logic
        /// Both specifications must be satisfied
        /// </summary>
        public Specification<T> And(Specification<T> other)
        {
            return new AndSpecification<T>(this, other);
        }

        /// <summary>
        /// Combine this specification with another using OR logic
        /// At least one specification must be satisfied
        /// </summary>
        public Specification<T> Or(Specification<T> other)
        {
            return new OrSpecification<T>(this, other);
        }

        /// <summary>
        /// Negate this specification using NOT logic
        /// Entity must NOT satisfy this specification
        /// </summary>
        public Specification<T> Not()
        {
            return new NotSpecification<T>(this);
        }

        /// <summary>
        /// Implicit conversion to Expression for easy usage
        /// </summary>
        public static implicit operator Expression<Func<T, bool>>(Specification<T> specification)
        {
            return specification.ToExpression();
        }
    }

    // ==================== COMPOSITE SPECIFICATIONS ====================

    /// <summary>
    /// AND specification - combines two specifications with AND logic
    /// </summary>
    internal class AndSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public AndSpecification(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var leftExpression = _left.ToExpression();
            var rightExpression = _right.ToExpression();

            // Combine expressions using AND
            var parameter = Expression.Parameter(typeof(T));
            var combined = Expression.AndAlso(
                Expression.Invoke(leftExpression, parameter),
                Expression.Invoke(rightExpression, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
    }

    /// <summary>
    /// OR specification - combines two specifications with OR logic
    /// </summary>
    internal class OrSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _left;
        private readonly Specification<T> _right;

        public OrSpecification(Specification<T> left, Specification<T> right)
        {
            _left = left;
            _right = right;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var leftExpression = _left.ToExpression();
            var rightExpression = _right.ToExpression();

            // Combine expressions using OR
            var parameter = Expression.Parameter(typeof(T));
            var combined = Expression.OrElse(
                Expression.Invoke(leftExpression, parameter),
                Expression.Invoke(rightExpression, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(combined, parameter);
        }
    }

    /// <summary>
    /// NOT specification - negates a specification
    /// </summary>
    internal class NotSpecification<T> : Specification<T>
    {
        private readonly Specification<T> _specification;

        public NotSpecification(Specification<T> specification)
        {
            _specification = specification;
        }

        public override Expression<Func<T, bool>> ToExpression()
        {
            var expression = _specification.ToExpression();

            // Negate expression using NOT
            var parameter = Expression.Parameter(typeof(T));
            var negated = Expression.Not(Expression.Invoke(expression, parameter));

            return Expression.Lambda<Func<T, bool>>(negated, parameter);
        }
    }
}
