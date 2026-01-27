using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a business rule is violated.
    /// </summary>
    public class BusinessRuleViolationException : DomainException
    {
        public string PropertyName { get; }
        public BusinessRuleViolationException(string message, string propertyName)
            : base(message)
        {
            PropertyName = propertyName;
        }
    }
}
