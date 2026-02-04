using LMS.Domain.Common;
using LMS.Domain.Events;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Events.Handlers
{
    /// <summary>
    /// Handler for EnrollmentCreatedEvent
    /// Handles actions that should occur when a user enrolls in a course
    /// </summary>
    public class EnrollmentCreatedEventHandler : IDomainEventHandler<EnrollmentCreatedEvent>
    {
        private readonly ILogger<EnrollmentCreatedEventHandler> _logger;

        public EnrollmentCreatedEventHandler(ILogger<EnrollmentCreatedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(EnrollmentCreatedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            // Log the enrollment
            _logger.LogInformation(
                "User {UserId} enrolled in course {CourseId} at {EnrollmentDate}",
                domainEvent.UserId,
                domainEvent.CourseId,
                domainEvent.EnrollmentDate
            );

            // TODO: Add additional actions here:
            // - Send welcome email with course details
            // - Create initial progress tracking record
            // - Update course enrollment count/statistics
            // - Notify instructor of new student
            // - Add to student's learning dashboard
            // - Send push notification to mobile app

            await Task.CompletedTask;
        }
    }
}
