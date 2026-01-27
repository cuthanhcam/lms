using SimpleLMS.Domain.Common;
using SimpleLMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Domain.Entities
{
    public class Lesson : BaseEntity
    {
        public string Title { get; private set; }
        public string? Content { get; private set; }
        public string? VideoUrl { get; private set; }
        public int Order { get; private set; }
        public int DurationMinutes { get; private set; }
        public Guid CourseId { get; private set; }

        // Navigation properties
        public Course? Course { get; private set; }

        // Constructor for EF Core
        private Lesson() { }

        public Lesson(string title, string? content, string? videoUrl, int order, int durationMinutes, Guid courseId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new BusinessRuleViolationException("Title cannot be null or empty.", nameof(title));
            if (order < 1)
                throw new BusinessRuleViolationException("Order must be greater than zero.", nameof(order));
            if (durationMinutes < 0)
                throw new BusinessRuleViolationException("Duration must be non-negative.", nameof(durationMinutes));
            if (courseId == Guid.Empty)
                throw new BusinessRuleViolationException("CourseId cannot be an empty GUID.", nameof(courseId));

            Title = title;
            Content = content;
            VideoUrl = videoUrl;
            Order = order;
            DurationMinutes = durationMinutes;
            CourseId = courseId;
        }

        // Update info for the lesson
        public void UpdateInfo(string title, string? content, string? videoUrl, int order, int durationMinutes)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new BusinessRuleViolationException("Title cannot be null or empty.", nameof(title));
            if (order < 1)
                throw new BusinessRuleViolationException("Order must be greater than zero.", nameof(order));
            if (durationMinutes < 0)
                throw new BusinessRuleViolationException("Duration must be non-negative.", nameof(durationMinutes));
            
            Title = title;
            Content = content;
            VideoUrl = videoUrl;
            Order = order;
            DurationMinutes = durationMinutes;
            MarkAsModified();
        }

        // Change order of the lesson
        public void ChangeOrder(int newOrder)
        {
            if (newOrder < 1)
                throw new BusinessRuleViolationException("Order must be greater than zero.", nameof(newOrder));
           
            Order = newOrder;
            MarkAsModified();
        }
    }
}
