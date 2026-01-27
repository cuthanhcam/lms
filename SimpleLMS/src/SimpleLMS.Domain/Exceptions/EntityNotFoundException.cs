using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when a business rule is violated.
    /// </summary>
    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string entityName, Guid entityId)
            : base($"The entity '{entityName}' with ID '{entityId}' was not found.")
        {
        }
    }
}
