using LMS.Domain.Events;

namespace LMS.Domain.Common
{
    /// <summary>
    /// Base class for all entities in the domain
    /// Entity is defined by its unique identifier (Id), not by its attributes
    /// Two entities are equal if they have the same Id, even if their attributes differ
    /// 
    /// This base class also provides domain event support:
    /// - Entities can raise domain events
    /// - Events are stored until explicitly dispatched
    /// - This enables event-driven architecture within the domain
    /// </summary>
    public abstract class Entity
    {
        private readonly List<DomainEvent> _domainEvents = new();

        /// <summary>
        /// Unique identifier for the entity
        /// Protected set ensures only the entity itself can change its ID
        /// </summary>
        public Guid Id { get; protected set; }

        /// <summary>
        /// Read-only collection of domain events raised by this entity
        /// Events are added via AddDomainEvent() and cleared via ClearDomainEvents()
        /// </summary>
        public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        /// <summary>
        /// Add a domain event to this entity
        /// Events will be dispatched when the entity is saved (typically in SaveChangesAsync)
        /// </summary>
        /// <param name="domainEvent">Event to add</param>
        protected void AddDomainEvent(DomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        /// <summary>
        /// Clear all domain events from this entity
        /// Called after events have been dispatched
        /// </summary>
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        // ==================== EQUALITY ====================

        /// <summary>
        /// Equality comparison based on Id
        /// Two entities are equal if they have the same type and same Id
        /// </summary>
        public override bool Equals(object? obj)
        {
            if (obj is not Entity other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            return Id == other.Id;
        }

        /// <summary>
        /// Hash code based on Id
        /// </summary>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        /// <summary>
        /// Operator overload for equality
        /// </summary>
        public static bool operator ==(Entity? left, Entity? right)
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
        public static bool operator !=(Entity? left, Entity? right)
        {
            return !(left == right);
        }
    }
}

