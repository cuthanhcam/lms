using SimpleLMS.Domain.Common;
using SimpleLMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace SimpleLMS.Domain.Entities
{
    public class Course : BaseEntity
    {
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public decimal Price { get; private set; }
        public Guid InstructorId { get; private set; }
        public bool IsPublished { get; private set; }

        // Navigation properties
        public User? Instructor { get; private set; }

        private readonly List<Lesson> _lessons = new();
        public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

        private readonly List<Enrollment> _enrollments = new();
        public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

        // Constructor for EF Core
        private Course() { }

        public Course(string title, string? description, decimal price, Guid instructorId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new BusinessRuleViolationException("Title cannot be empty.", nameof(title));
            if (price < 0)
                throw new BusinessRuleViolationException("Price cannot be negative.", nameof(price));
            if (instructorId == Guid.Empty)
                throw new BusinessRuleViolationException("InstructorId cannot be empty.", nameof(instructorId));

            Title = title;
            Description = description;
            Price = price;
            InstructorId = instructorId;
            IsPublished = false;
        }
        
        public void UpdateInfo(string title, string? description, decimal price)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new BusinessRuleViolationException("Title cannot be empty.", nameof(title));
            if (price < 0)
                throw new BusinessRuleViolationException("Price cannot be negative.", nameof(price));
            
            Title = title;
            Description = description;
            Price = price;
            MarkAsModified();
        }

        // Publish the course
        // Business rule: A course can only be published if it has at least one lesson.
        public void Publish()
        {
            if (IsPublished)
                throw new BusinessRuleViolationException("Course is already published.", nameof(IsPublished));
            if (IsDeleted)
                throw new BusinessRuleViolationException("Cannot publish a deleted course.", nameof(IsDeleted));
            if (_lessons.Count == 0)
                throw new BusinessRuleViolationException("Cannot publish a course with no lessons.", nameof(_lessons));

            IsPublished = true;
            MarkAsModified();
        }

        // Unpublish the course
        public void Unpublish()
        {
            if (!IsPublished)
                throw new BusinessRuleViolationException("Course is not published.", nameof(IsPublished));
            if (IsDeleted)
                throw new BusinessRuleViolationException("Cannot unpublish a deleted course.", nameof(IsDeleted));
            
            IsPublished = false;
            MarkAsModified();
        }

        // Add a lesson to the course
        public void AddLesson(Lesson lesson)
        {
            if (IsDeleted)
                throw new BusinessRuleViolationException("Cannot add a lesson to a deleted course.", nameof(IsDeleted));
            if (lesson == null)
                throw new BusinessRuleViolationException("Lesson cannot be null.", nameof(lesson));

            _lessons.Add(lesson);
            MarkAsModified();
        }
        // Remove lesson from the course
        public void RemoveLesson(Lesson lesson)
        {
            if (IsDeleted)
                throw new BusinessRuleViolationException("Cannot remove a lesson from a deleted course.", nameof(IsDeleted));
            if (lesson == null)
                throw new BusinessRuleViolationException("Lesson cannot be null.", nameof(lesson));
            if (!_lessons.Contains(lesson))
                throw new BusinessRuleViolationException("Lesson does not exist in the course.", nameof(lesson));
            
            _lessons.Remove(lesson);
            MarkAsModified();
        }
        // Override Delete method to enforce business rules
        public override void Delete()
        {
            if (IsPublished)
                throw new BusinessRuleViolationException("Cannot delete a published course.", nameof(IsPublished));
            if (IsDeleted)
                throw new BusinessRuleViolationException("Course is already deleted.", nameof(IsDeleted));
            
            base.Delete();
        }
    }
}
