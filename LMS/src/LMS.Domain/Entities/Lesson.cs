using LMS.Domain.Common;
using LMS.Domain.Exceptions;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// Lesson Entity - represents a lesson within a course
    /// 
    /// Lesson is NOT an Aggregate Root - it belongs to Course aggregate
    /// This means:
    /// - Lessons should be accessed through Course
    /// - Lessons are created/modified via Course methods
    /// - Course maintains consistency of its lessons
    /// 
    /// Business Rules:
    /// - Lesson must belong to a course
    /// - Lesson title cannot be empty
    /// - Lesson content cannot be empty
    /// - Lesson order must be positive
    /// </summary>
    public class Lesson : Entity
    {
        // ==================== PROPERTIES ====================

        /// <summary>
        /// ID of the course this lesson belongs to
        /// </summary>
        public Guid CourseId { get; private set; }

        /// <summary>
        /// Navigation property to parent course
        /// </summary>
        public Course? Course { get; private set; }

        /// <summary>
        /// Lesson title - required
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Lesson content/body - required
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        /// Display order within the course
        /// Used to sort lessons in the correct sequence
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// Soft delete flag
        /// </summary>
        public bool IsDeleted { get; private set; }

        // ==================== CONSTRUCTORS ====================

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private Lesson()
        {
            Title = string.Empty;
            Content = string.Empty;
        }

        /// <summary>
        /// Private constructor with parameters
        /// Only factory method can create Lesson
        /// </summary>
        private Lesson(string title, string content, int order)
        {
            Title = title;
            Content = content;
            Order = order;
            IsDeleted = false;
            Id = Guid.NewGuid();
        }

        // ==================== FACTORY METHODS ====================

        /// <summary>
        /// Factory method to create a new Lesson
        /// 
        /// Note: Lesson is created without CourseId initially
        /// CourseId is set when lesson is added to a Course via AddLesson()
        /// </summary>
        /// <param name="title">Lesson title - must not be empty</param>
        /// <param name="content">Lesson content - must not be empty</param>
        /// <param name="order">Display order - must be positive</param>
        /// <returns>Valid Lesson object</returns>
        public static Lesson Create(string title, string content, int order)
        {
            // Validate title
            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Lesson title cannot be empty");

            // Validate content
            if (string.IsNullOrWhiteSpace(content))
                throw new DomainException("Lesson content cannot be empty");

            // Validate order
            if (order <= 0)
                throw new DomainException("Lesson order must be positive");

            return new Lesson(title, content, order);
        }

        // ==================== DOMAIN METHODS ====================

        /// <summary>
        /// Assign this lesson to a course
        /// Called by Course.AddLesson() method
        /// 
        /// This method should only be called by Course aggregate
        /// </summary>
        internal void AssignToCourse(Guid courseId)
        {
            if (courseId == Guid.Empty)
                throw new DomainException("Course ID cannot be empty");

            if (CourseId != Guid.Empty && CourseId != courseId)
                throw new DomainException("Lesson is already assigned to another course");

            CourseId = courseId;
        }

        /// <summary>
        /// Update lesson details
        /// 
        /// Business rules:
        /// - Cannot update deleted lesson
        /// - Title cannot be empty
        /// - Content cannot be empty
        /// - Order must be positive
        /// </summary>
        public void UpdateDetails(string title, string content, int order)
        {
            if (IsDeleted)
                throw new DomainException("Cannot update deleted lesson");

            if (string.IsNullOrWhiteSpace(title))
                throw new DomainException("Lesson title cannot be empty");

            if (string.IsNullOrWhiteSpace(content))
                throw new DomainException("Lesson content cannot be empty");

            if (order <= 0)
                throw new DomainException("Lesson order must be positive");

            Title = title;
            Content = content;
            Order = order;
        }

        /// <summary>
        /// Change the order of this lesson
        /// Used when reordering lessons in a course
        /// </summary>
        public void ChangeOrder(int newOrder)
        {
            if (newOrder <= 0)
                throw new DomainException("Lesson order must be positive");

            Order = newOrder;
        }

        /// <summary>
        /// Soft delete the lesson
        /// </summary>
        public void Delete()
        {
            if (IsDeleted)
                throw new DomainException("Lesson is already deleted");

            IsDeleted = true;
        }

        /// <summary>
        /// Restore a deleted lesson
        /// </summary>
        public void Restore()
        {
            if (!IsDeleted)
                throw new DomainException("Lesson is not deleted");

            IsDeleted = false;
        }

        /// <summary>
        /// Check if this lesson belongs to a specific course
        /// </summary>
        public bool BelongsToCourse(Guid courseId)
        {
            return CourseId == courseId;
        }
    }
}

