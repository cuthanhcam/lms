namespace LMS.Domain.Events
{
    /// <summary>
    /// Base class for all domain events
    /// 
    /// Domain Events represent something that happened in the domain that domain experts care about.
    /// They are used to make implicit concepts in the domain explicit.
    /// 
    /// Benefits of Domain Events:
    /// 1. Decoupling: Events decouple domain logic from side effects
    /// 2. Audit Trail: Events provide a history of what happened
    /// 3. Integration: Events can trigger actions in other bounded contexts
    /// 4. Temporal Decoupling: Event handlers can run asynchronously
    /// 
    /// Example scenarios:
    /// - CoursePublished → Send email to followers
    /// - UserRegistered → Create welcome message
    /// - EnrollmentCreated → Update course statistics
    /// </summary>
    public abstract class DomainEvent
    {
        /// <summary>
        /// When the event occurred
        /// Set automatically when event is created
        /// </summary>
        public DateTime OccurredOn { get; }

        /// <summary>
        /// Unique identifier for this event
        /// Useful for idempotency and tracking
        /// </summary>
        public Guid EventId { get; }

        protected DomainEvent()
        {
            OccurredOn = DateTime.UtcNow;
            EventId = Guid.NewGuid();
        }
    }
}
