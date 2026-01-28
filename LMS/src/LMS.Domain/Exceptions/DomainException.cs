namespace LMS.Domain.Exceptions
{
    /// <summary>
    /// Base exception for all domain-related errors
    /// 
    /// Domain exceptions are thrown when domain invariants are violated
    /// These represent business rule violations that should be handled by the application layer
    /// 
    /// Examples:
    /// - "Cannot publish course without lessons"
    /// - "Course price cannot be negative"
    /// - "Cannot enroll in unpublished course"
    /// 
    /// These are different from technical exceptions (NullReferenceException, etc)
    /// </summary>
    public class DomainException : Exception
    {
        /// <summary>
        /// Create a domain exception with a message
        /// </summary>
        /// <param name="message">Description of what business rule was violated</param>
        public DomainException(string message) : base(message)
        {
        }

        /// <summary>
        /// Create a domain exception with a message and inner exception
        /// </summary>
        public DomainException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}
