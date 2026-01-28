namespace LMS.Domain.Common
{
    /// <summary>
    /// Base class for all Value Objects in the domain
    /// Value Object is defined by its attributes, not by a unique identifier
    /// Two value objects are equal if all their attributes are equal
    /// Value Objects are immutable - once created, they cannot be changed
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Get all components that define equality for this value object
        /// Each derived class must implement this to return all properties that matter for equality
        /// </summary>
        protected abstract IEnumerable<object?> GetEqualityComponents();

        /// <summary>
        /// Equality comparison based on all components
        /// Two value objects are equal if all their components are equal
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
                return false;

            var other = (ValueObject)obj;

            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Hash code based on all components
        /// </summary>
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x?.GetHashCode() ?? 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <summary>
        /// Operator overload for equality
        /// </summary>
        public static bool operator ==(ValueObject? left, ValueObject? right)
        {
            if (left is null && right is null)
                return true;

            if (left is null || right is null)
                return false;

            return left.Equals(right);
        }

        /// <summary>
        /// Operator overload for inequality
        /// </summary>
        public static bool operator !=(ValueObject? left, ValueObject? right)
        {
            return !(left == right);
        }
    }
}
