using LMS.Domain.Common;
using LMS.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace LMS.Domain.ValueObjects
{
    /// <summary>
    /// Email Value Object - represents a valid email address
    /// 
    /// Value Object characteristics:
    /// 1. Immutable - once created, cannot be changed
    /// 2. No identity - two emails are equal if their values are equal
    /// 3. Self-validating - always in a valid state
    /// 4. Side-effect free - methods don't change state
    /// 
    /// Benefits of using Email instead of string:
    /// - Type safety: cannot accidentally pass a name where email is expected
    /// - Always valid: Email object cannot exist in invalid state
    /// - Business concept: clearly expresses the domain concept
    /// - Reusable validation: validation logic in one place
    /// </summary>
    public class Email : ValueObject
    {
        /// <summary>
        /// The email address value
        /// Private set ensures immutability
        /// </summary>
        public string Value { get; private set; }

        /// <summary>
        /// Private constructor - prevents direct instantiation
        /// Use Create() factory method instead
        /// This ensures Email can only be created through validation
        /// </summary>
        private Email(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Factory method to create a valid Email
        /// This is the only way to create an Email object
        /// 
        /// Throws DomainException if validation fails
        /// </summary>
        /// <param name="email">Email address string</param>
        /// <returns>Valid Email object</returns>
        public static Email Create(string email)
        {
            // Validate email is not empty
            if (string.IsNullOrWhiteSpace(email))
                throw new DomainException("Email cannot be empty");

            // Normalize email to lowercase
            email = email.Trim().ToLowerInvariant();

            // Validate email format
            if (!IsValidEmailFormat(email))
                throw new DomainException($"'{email}' is not a valid email address");

            // Validate email length (RFC 5321)
            if (email.Length > 254)
                throw new DomainException("Email address is too long (max 254 characters)");

            return new Email(email);
        }

        /// <summary>
        /// Validate email format using regex
        /// Basic validation following common email format rules
        /// </summary>
        private static bool IsValidEmailFormat(string email)
        {
            // Simple but effective email regex pattern
            // More complex patterns exist but this covers 99% of cases
            var emailRegex = new Regex(
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            return emailRegex.IsMatch(email);
        }

        /// <summary>
        /// Get equality components for ValueObject base class
        /// Email is equal to another Email if they have the same Value
        /// </summary>
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        /// <summary>
        /// String representation of the email
        /// </summary>
        public override string ToString() => Value;

        /// <summary>
        /// Implicit conversion from Email to string
        /// Allows: string emailString = emailObject;
        /// </summary>
        public static implicit operator string(Email email) => email.Value;
    }
}
