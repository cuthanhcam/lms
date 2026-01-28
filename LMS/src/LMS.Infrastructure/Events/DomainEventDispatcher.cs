using LMS.Domain.Common;
using LMS.Domain.Events;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Events
{
    /// <summary>
    /// Domain event dispatcher implementation
    /// 
    /// This implementation uses IServiceProvider to resolve event handlers dynamically.
    /// In a production system, you might want to use MediatR or similar library.
    /// 
    /// Responsibilities:
    /// 1. Find all handlers for a given event type
    /// 2. Execute handlers in sequence
    /// 3. Handle exceptions during event processing
    /// 4. Log event dispatching activity
    /// </summary>
    public class DomainEventDispatcher : IDomainEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DomainEventDispatcher> _logger;

        public DomainEventDispatcher(
            IServiceProvider serviceProvider,
            ILogger<DomainEventDispatcher> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        /// <summary>
        /// Dispatch a single domain event to all registered handlers
        /// </summary>
        public async Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
            where TEvent : DomainEvent
        {
            if (domainEvent == null)
                throw new ArgumentNullException(nameof(domainEvent));

            _logger.LogInformation(
                "Dispatching domain event: {EventType} with ID: {EventId}",
                domainEvent.GetType().Name,
                domainEvent.EventId);

            try
            {
                // Resolve all handlers for this event type
                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
                var handlers = _serviceProvider.GetServices(handlerType);

                // Execute each handler in sequence
                foreach (var handler in handlers)
                {
                    if (handler == null) continue;

                    var handleMethod = handlerType.GetMethod("HandleAsync");
                    if (handleMethod != null)
                    {
                        var task = (Task)handleMethod.Invoke(handler, new object[] { domainEvent, cancellationToken })!;
                        await task;

                        _logger.LogInformation(
                            "Successfully executed handler {HandlerType} for event {EventType}",
                            handler.GetType().Name,
                            domainEvent.GetType().Name);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error occurred while dispatching domain event: {EventType} with ID: {EventId}",
                    domainEvent.GetType().Name,
                    domainEvent.EventId);

                // In production, you might want to:
                // 1. Store failed events for retry
                // 2. Send to dead letter queue
                // 3. Trigger compensating actions
                throw;
            }
        }

        /// <summary>
        /// Dispatch multiple domain events in sequence
        /// </summary>
        public async Task DispatchAsync(IEnumerable<DomainEvent> domainEvents, CancellationToken cancellationToken = default)
        {
            if (domainEvents == null)
                throw new ArgumentNullException(nameof(domainEvents));

            foreach (var domainEvent in domainEvents)
            {
                // Use reflection to call the generic DispatchAsync method
                var dispatchMethod = GetType()
                    .GetMethod(nameof(DispatchAsync), new[] { domainEvent.GetType(), typeof(CancellationToken) });

                if (dispatchMethod != null)
                {
                    var genericMethod = dispatchMethod.MakeGenericMethod(domainEvent.GetType());
                    var task = (Task)genericMethod.Invoke(this, new object[] { domainEvent, cancellationToken })!;
                    await task;
                }
            }
        }
    }
}
