using LMS.Domain.Events;

namespace LMS.Domain.Common
{
    /// <summary>
    /// Interface for handling domain events
    /// 
    /// Implement this interface for each type of domain event you want to handle.
    /// The handler contains the logic that should execute when an event occurs.
    /// 
    /// Example: CoursePublishedEventHandler implements IDomainEventHandler&lt;CoursePublishedEvent&gt;
    /// </summary>
    /// <typeparam name="TEvent">Type of domain event to handle</typeparam>
    public interface IDomainEventHandler<in TEvent> where TEvent : DomainEvent
    {
        /// <summary>
        /// Handle the domain event asynchronously
        /// </summary>
        /// <param name="domainEvent">The event to handle</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
    }
}
