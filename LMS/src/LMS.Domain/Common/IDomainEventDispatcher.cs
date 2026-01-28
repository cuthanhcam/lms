using LMS.Domain.Events;

namespace LMS.Domain.Common
{
    /// <summary>
    /// Interface for dispatching domain events
    /// 
    /// The domain event dispatcher is responsible for:
    /// 1. Publishing events to all registered handlers
    /// 2. Managing the event handling pipeline
    /// 3. Ensuring events are processed reliably
    /// 
    /// Implementation will be in Infrastructure layer (e.g., using MediatR)
    /// </summary>
    public interface IDomainEventDispatcher
    {
        /// <summary>
        /// Dispatch a domain event to all registered handlers
        /// </summary>
        /// <typeparam name="TEvent">Type of domain event</typeparam>
        /// <param name="domainEvent">The event to dispatch</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : DomainEvent;

        /// <summary>
        /// Dispatch multiple domain events in sequence
        /// </summary>
        /// <param name="domainEvents">Collection of events to dispatch</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task DispatchAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default);
    }
}
