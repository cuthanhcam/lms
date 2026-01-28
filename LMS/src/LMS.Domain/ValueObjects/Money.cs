using LMS.Domain.Common;
using LMS.Domain.Exceptions;

namespace LMS.Domain.ValueObjects
{
    /// <summary>
    /// Money Value Object - represents a monetary amount with currency
    /// 
    /// Benefits of Money Value Object:
    /// - Type safety: cannot add money with different currencies
    /// - Business concept: clearly expresses monetary value
    /// - Validation: ensures amount is never negative (for prices)
    /// - Operations: can add, subtract money safely
    /// 
    /// In DDD, Money is a classic example of a Value Object
    /// </summary>
    public class Money : ValueObject
    {
        /// <summary>
        /// The monetary amount
        /// Private set ensures immutability
        /// </summary>
        public decimal Amount { get; private set; }

        /// <summary>
        /// The currency code (USD, EUR, VND, etc.)
        /// Following ISO 4217 standard
        /// </summary>
        public string Currency { get; private set; }

        /// <summary>
        /// Private constructor - prevents direct instantiation
        /// Use Create() factory method instead
        /// </summary>
        private Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        /// <summary>
        /// Factory method to create Money for prices (non-negative amounts)
        /// Most common use case in LMS system
        /// </summary>
        /// <param name="amount">Amount of money</param>
        /// <param name="currency">Currency code (default: USD)</param>
        /// <returns>Valid Money object</returns>
        public static Money Create(decimal amount, string currency = "USD")
        {
            // Validate amount is not negative (for prices)
            if (amount < 0)
                throw new DomainException("Money amount cannot be negative");

            // Validate currency code
            if (string.IsNullOrWhiteSpace(currency))
                throw new DomainException("Currency code cannot be empty");

            // Normalize currency to uppercase
            currency = currency.Trim().ToUpperInvariant();

            // Validate currency code length (ISO 4217 = 3 characters)
            if (currency.Length != 3)
                throw new DomainException("Currency code must be 3 characters (ISO 4217)");

            return new Money(amount, currency);
        }

        /// <summary>
        /// Create zero money with specified currency
        /// Useful as default value
        /// </summary>
        public static Money Zero(string currency = "USD") => new Money(0, currency);

        /// <summary>
        /// Create Money allowing negative amounts (for refunds, discounts, etc.)
        /// Use this for scenarios where negative amounts are valid
        /// </summary>
        public static Money CreateAllowNegative(decimal amount, string currency = "USD")
        {
            if (string.IsNullOrWhiteSpace(currency))
                throw new DomainException("Currency code cannot be empty");

            currency = currency.Trim().ToUpperInvariant();

            if (currency.Length != 3)
                throw new DomainException("Currency code must be 3 characters (ISO 4217)");

            return new Money(amount, currency);
        }

        /// <summary>
        /// Add two Money values
        /// Can only add money with the same currency
        /// </summary>
        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Cannot add money with different currencies: {Currency} and {other.Currency}");

            return new Money(Amount + other.Amount, Currency);
        }

        /// <summary>
        /// Subtract two Money values
        /// Can only subtract money with the same currency
        /// </summary>
        public Money Subtract(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Cannot subtract money with different currencies: {Currency} and {other.Currency}");

            return new Money(Amount - other.Amount, Currency);
        }

        /// <summary>
        /// Multiply money by a factor
        /// Useful for calculating discounts, taxes, etc.
        /// </summary>
        public Money Multiply(decimal factor)
        {
            return new Money(Amount * factor, Currency);
        }

        /// <summary>
        /// Check if this money is greater than another
        /// </summary>
        public bool IsGreaterThan(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Cannot compare money with different currencies: {Currency} and {other.Currency}");

            return Amount > other.Amount;
        }

        /// <summary>
        /// Check if this money is less than another
        /// </summary>
        public bool IsLessThan(Money other)
        {
            if (Currency != other.Currency)
                throw new DomainException($"Cannot compare money with different currencies: {Currency} and {other.Currency}");

            return Amount < other.Amount;
        }

        /// <summary>
        /// Check if this is zero money
        /// </summary>
        public bool IsZero() => Amount == 0;

        /// <summary>
        /// Check if this is positive money
        /// </summary>
        public bool IsPositive() => Amount > 0;

        /// <summary>
        /// Get equality components for ValueObject base class
        /// Money is equal if both Amount and Currency are equal
        /// </summary>
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

        /// <summary>
        /// String representation of money
        /// Format: "100.00 USD"
        /// </summary>
        public override string ToString() => $"{Amount:F2} {Currency}";

        /// <summary>
        /// Operator overload for addition
        /// Allows: money1 + money2
        /// </summary>
        public static Money operator +(Money left, Money right) => left.Add(right);

        /// <summary>
        /// Operator overload for subtraction
        /// Allows: money1 - money2
        /// </summary>
        public static Money operator -(Money left, Money right) => left.Subtract(right);

        /// <summary>
        /// Operator overload for multiplication
        /// Allows: money * factor
        /// </summary>
        public static Money operator *(Money money, decimal factor) => money.Multiply(factor);

        /// <summary>
        /// Operator overload for greater than comparison
        /// </summary>
        public static bool operator >(Money left, Money right) => left.IsGreaterThan(right);

        /// <summary>
        /// Operator overload for less than comparison
        /// </summary>
        public static bool operator <(Money left, Money right) => left.IsLessThan(right);

        /// <summary>
        /// Operator overload for greater than or equal comparison
        /// </summary>
        public static bool operator >=(Money left, Money right) => 
            left.IsGreaterThan(right) || left == right;

        /// <summary>
        /// Operator overload for less than or equal comparison
        /// </summary>
        public static bool operator <=(Money left, Money right) => 
            left.IsLessThan(right) || left == right;
    }
}
