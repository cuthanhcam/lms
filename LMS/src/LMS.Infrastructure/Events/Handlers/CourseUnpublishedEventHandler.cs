using LMS.Domain.Common;
using LMS.Domain.Events;
using Microsoft.Extensions.Logging;

namespace LMS.Infrastructure.Events.Handlers
{
    /// <summary>
    /// Handler for CourseUnpublishedEvent.
    /// </summary>
    public class CourseUnpublishedEventHandler : IDomainEventHandler<CourseUnpublishedEvent>
    {
        private readonly ILogger<CourseUnpublishedEventHandler> _logger;

        public CourseUnpublishedEventHandler(ILogger<CourseUnpublishedEventHandler> logger)
        {
            _logger = logger;
        }

        public async Task HandleAsync(CourseUnpublishedEvent domainEvent, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                "Course unpublished: {CourseId} - '{CourseTitle}' by instructor {InstructorId}",
                domainEvent.CourseId,
                domainEvent.CourseTitle,
                domainEvent.InstructorId);

            await Task.CompletedTask;
        }
    }
}
