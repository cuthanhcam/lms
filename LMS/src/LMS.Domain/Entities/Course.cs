using LMS.Domain.Common;
using LMS.Domain.Events;
using LMS.Domain.Exceptions;
using LMS.Domain.ValueObjects;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Course Aggregate Root - represents a learning course in the system
    /// 
    /// As an Aggregate Root, Course is responsible for:
    /// 1. Maintaining its own invariants (business rules)
    /// 2. Managing child entities (Lessons) - lessons can only be added/removed through Course
    /// 3. Exposing domain methods instead of property setters
    /// 4. Raising domain events when important state changes occur
    /// 
    /// Business Rules (Invariants):
    /// - Course title cannot be empty
    /// - Course price cannot be negative
    /// - Published course must have at least one lesson
    /// - Deleted course cannot be published
    /// - Lessons can only be added to non-deleted courses
    /// </summary>
    public class Course : Entity, IAggregateRoot
    {
        // ==================== PROPERTIES ====================
        // Private setters prevent external code from directly modifying state
        // State can only be changed through domain methods that enforce business rules

        /// <summary>
        /// Course title - required, non-empty
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Course description - optional
        /// </summary>
        public string? Description { get; private set; }

        /// <summary>
        /// Course price as Money Value Object
        /// Ensures price is always valid and has currency
        /// </summary>
        public Money Price { get; private set; }

        /// <summary>
        /// ID of the user who created this course (instructor)
        /// </summary>
        public Guid CreatedBy { get; private set; }

        /// <summary>
        /// Navigation property to the creator
        /// </summary>
        public User? CreatedByUser { get; private set; }

        /// <summary>
        /// Whether this course is published and visible to students
        /// </summary>
        public bool IsPublished { get; private set; }

        /// <summary>
        /// Soft delete flag - deleted courses are not shown but kept for data integrity
        /// </summary>
        public bool IsDeleted { get; private set; }

        /// <summary>
        /// When this course was created
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Private backing field for lessons collection
        /// Prevents external code from modifying the collection directly
        /// </summary>
        private readonly List<Lesson> _lessons = new();

        /// <summary>
        /// Read-only access to lessons
        /// External code can read but cannot add/remove lessons directly
        /// Must use AddLesson() or RemoveLesson() methods
        /// </summary>
        public IReadOnlyCollection<Lesson> Lessons => _lessons.AsReadOnly();

        /// <summary>
        /// Private backing field for enrollments collection
        /// </summary>
        private readonly List<Enrollment> _enrollments = new();

        /// <summary>
        /// Read-only access to enrollments
        /// </summary>
        public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

        // ==================== CONSTRUCTORS ====================

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// EF Core requires a parameterless constructor to create entities from database
        /// Private ensures it cannot be used by domain code
        /// </summary>
        private Course()
        {
            // Required by EF Core
            Title = string.Empty;
            Price = Money.Zero();
        }

        /// <summary>
        /// Private constructor with parameters
        /// Only factory methods can call this constructor
        /// Ensures all Course objects are created through validated factory methods
        /// </summary>
        private Course(string title, string? description, Money price, Guid createdBy)
        {
            Title = title;
            Description = description;
            Price = price;
            CreatedBy = createdBy;
            IsPublished = false;
            IsDeleted = false;
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        // ==================== FACTORY METHODS ====================

        /// <summary>
        /// Factory method to create a new Course
        /// This is the ONLY way to create a Course from domain code
        /// 
        /// Validates all business rules before creating the course
        /// </summary>
        /// <param name="title">Course title - must not be empty</param>
        /// <param name="description">Course description - optional</param>
        /// <param name="price">Course price - must not be negative</param>
        /// <param name="createdBy">ID of user creating the course</param>
        /// <returns>Valid Course object</returns>
        public static Course Create(string title, string? description, Money price, Guid createdBy)
        {
            // Validate title
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Course title cannot be empty");

            // Validate price (Money Value Object already validates it's not negative)
            if (!price.IsPositive() && !price.IsZero())
                throw new DomainException("Course price must be zero or positive");

            // Validate creator ID
            if (createdBy == Guid.Empty)
                throw new DomainException("Course must have a valid creator");

            return new Course(title, description, price, createdBy);
        }

        // ==================== DOMAIN METHODS ====================
        // These methods encapsulate business logic and enforce invariants

        /// <summary>
        /// Update course details (title, description, price)
        /// 
        /// Business rules:
        /// - Cannot update deleted course
        /// - Title cannot be empty
        /// - Price must be valid
        /// </summary>
        public void UpdateDetails(string title, string? description, Money price)
        {
            // Check if course is deleted
            if (IsDeleted)
                throw new DomainException("Cannot update deleted course");

            // Validate title
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Course title cannot be empty");

            // Validate price
            if (!price.IsPositive() && !price.IsZero())
                throw new DomainException("Course price must be zero or positive");

            // Update properties
            Title = title;
            Description = description;
            Price = price;
        }

        /// <summary>
        /// Publish the course to make it visible to students
        /// 
        /// Business rules:
        /// - Course must have at least one lesson
        /// - Course must not be deleted
        /// - Course must not be already published
        /// </summary>
        public void Publish()
        {
            // Check if already published
            if (IsPublished)
                throw new DomainException("Course is already published");

            // Check if deleted
            if (IsDeleted)
                throw new DomainException("Cannot publish deleted course");

            // Check if has lessons
            if (!_lessons.Any())
                throw new DomainException("Cannot publish course without lessons");

            // Publish course
            IsPublished = true;

            // Raise domain event
            AddDomainEvent(new CoursePublishedEvent(
                courseId: Id,
                courseTitle: Title,
                instructorId: CreatedBy,
                lessonCount: _lessons.Count
            ));
        }

        /// <summary>
        /// Unpublish the course (make it invisible to students)
        /// Useful when course needs maintenance or updates
        /// </summary>
        public void Unpublish()
        {
            if (!IsPublished)
                throw new DomainException("Course is not published");

            IsPublished = false;

            // TODO: Raise domain event - CourseUnpublished
        }

        /// <summary>
        /// Add a lesson to this course
        /// 
        /// Business rules:
        /// - Cannot add lesson to deleted course
        /// - Lesson must be valid
        /// - Lesson order must be positive
        /// </summary>
        public void AddLesson(Lesson lesson)
        {
            if (IsDeleted)
                throw new DomainException("Cannot add lesson to deleted course");

            if (lesson == null)
                throw new DomainException("Lesson cannot be null");

            // Set the course reference in the lesson
            lesson.AssignToCourse(Id);

            _lessons.Add(lesson);
        }

        /// <summary>
        /// Remove a lesson from this course
        /// 
        /// Business rules:
        /// - Cannot remove lesson from published course (must unpublish first)
        /// - Lesson must exist in the course
        /// </summary>
        public void RemoveLesson(Guid lessonId)
        {
            if (IsPublished)
                throw new DomainException("Cannot remove lesson from published course. Unpublish the course first.");

            var lesson = _lessons.FirstOrDefault(l => l.Id == lessonId);
            if (lesson == null)
                throw new DomainException($"Lesson with ID {lessonId} not found in this course");

            _lessons.Remove(lesson);
        }

        /// <summary>
        /// Soft delete the course
        /// Course is marked as deleted but kept in database for data integrity
        /// 
        /// Business rules:
        /// - Cannot delete already deleted course
        /// - Cannot delete published course (must unpublish first)
        /// </summary>
        public void Delete()
        {
            if (IsDeleted)
                throw new DomainException("Course is already deleted");

            if (IsPublished)
                throw new DomainException("Cannot delete published course. Unpublish it first.");

            IsDeleted = true;

            // Count active enrollments before deletion
            var activeEnrollmentCount = _enrollments.Count(e => e.Status == EnrollmentStatus.Active);
            
            // Raise domain event
            AddDomainEvent(new CourseDeletedEvent(
                courseId: Id,
                courseTitle: Title,
                instructorId: CreatedBy,
                activeEnrollmentCount: activeEnrollmentCount
            ));
        }

        /// <summary>
        /// Restore a deleted course
        /// </summary>
        public void Restore()
        {
            if (!IsDeleted)
                throw new DomainException("Course is not deleted");

            IsDeleted = false;
        }

        /// <summary>
        /// Check if user is the owner of this course
        /// Useful for authorization checks
        /// </summary>
        public bool IsOwnedBy(Guid userId)
        {
            return CreatedBy == userId;
        }

        /// <summary>
        /// Check if course can be enrolled by students
        /// </summary>
        public bool CanBeEnrolled()
        {
            return IsPublished && !IsDeleted;
        }

        /// <summary>
        /// Get total number of lessons (for display purposes)
        /// </summary>
        public int GetLessonCount()
        {
            return _lessons.Count(l => !l.IsDeleted);
        }
    }
}

