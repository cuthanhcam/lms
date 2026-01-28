using LMS.Domain.Common;
using LMS.Domain.Exceptions;
using LMS.Domain.ValueObjects;

namespace LMS.Domain.Entities
{
    /// <summary>
    /// User Entity - represents a system user (student, instructor, or admin)
    /// 
    /// User is an Aggregate Root because:
    /// - It has independent lifecycle
    /// - It controls its own state and business rules
    /// - It manages authentication and authorization data
    /// 
    /// Business Rules:
    /// - Username must be unique and not empty
    /// - Email must be valid format
    /// - Password must be hashed (never store plain text)
    /// - Cannot delete admin users
    /// - Inactive users cannot authenticate
    /// - Created date is immutable
    /// </summary>
    public class User : Entity, IAggregateRoot
    {
        // ==================== BACKING FIELDS ====================

        private readonly List<Course> _createdCourses = new();
        private readonly List<Enrollment> _enrollments = new();

        // ==================== PROPERTIES ====================

        /// <summary>
        /// Unique username for login
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// User's email address - using Email Value Object
        /// Ensures email is always in valid format
        /// </summary>
        public Email Email { get; private set; }

        /// <summary>
        /// Hashed password - NEVER store plain text password
        /// Use proper hashing algorithm (BCrypt, Argon2, etc.)
        /// </summary>
        public string PasswordHash { get; private set; }

        /// <summary>
        /// User's role in the system
        /// Determines permissions and access levels
        /// </summary>
        public UserRole Role { get; private set; }

        /// <summary>
        /// Whether the user account is active
        /// Inactive users cannot login or access the system
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// When the user account was created
        /// Immutable - set once at creation
        /// </summary>
        public DateTime CreatedAt { get; private set; }

        /// <summary>
        /// Read-only collection of courses created by this user (for instructors)
        /// Use AddCourse() to add courses
        /// </summary>
        public IReadOnlyCollection<Course> CreatedCourses => _createdCourses.AsReadOnly();

        /// <summary>
        /// Read-only collection of enrollments (for students)
        /// Use EnrollInCourse() to enroll
        /// </summary>
        public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

        // ==================== CONSTRUCTORS ====================

        /// <summary>
        /// Private parameterless constructor for EF Core
        /// </summary>
        private User()
        {
            UserName = string.Empty;
            Email = ValueObjects.Email.Create("default@example.com");
            PasswordHash = string.Empty;
        }

        /// <summary>
        /// Private constructor with parameters
        /// Only factory method can create User
        /// </summary>
        private User(string userName, Email email, string passwordHash, UserRole role)
        {
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        // ==================== FACTORY METHODS ====================

        /// <summary>
        /// Factory method to create a new User
        /// 
        /// Business Rules:
        /// - Username cannot be empty
        /// - Email must be valid (enforced by Email value object)
        /// - Password must be already hashed
        /// </summary>
        /// <param name="userName">Username - must not be empty</param>
        /// <param name="email">Email value object - already validated</param>
        /// <param name="passwordHash">Hashed password - never plain text</param>
        /// <param name="role">User role</param>
        /// <returns>Valid User object</returns>
        public static User Create(string userName, Email email, string passwordHash, UserRole role = UserRole.Student)
        {
            // Validate username
            if (string.IsNullOrWhiteSpace(userName))
                throw new DomainException("Username cannot be empty");

            // Validate password hash
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new DomainException("Password hash cannot be empty");

            // Email is already validated by Email Value Object

            return new User(userName, email, passwordHash, role);
        }

        // ==================== DOMAIN METHODS ====================

        /// <summary>
        /// Update user profile information
        /// 
        /// Business rules:
        /// - Cannot update inactive user
        /// - Username cannot be empty
        /// - Email must be valid
        /// </summary>
        public void UpdateProfile(string userName, Email email)
        {
            if (!IsActive)
                throw new DomainException("Cannot update inactive user profile");

            if (string.IsNullOrWhiteSpace(userName))
                throw new DomainException("Username cannot be empty");

            UserName = userName;
            Email = email;
        }

        /// <summary>
        /// Change user's password
        /// 
        /// Business rules:
        /// - Cannot change password for inactive user
        /// - New password must be hashed
        /// </summary>
        public void ChangePassword(string newPasswordHash)
        {
            if (!IsActive)
                throw new DomainException("Cannot change password for inactive user");

            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new DomainException("Password hash cannot be empty");

            PasswordHash = newPasswordHash;
        }

        /// <summary>
        /// Change user's role
        /// 
        /// Business rules:
        /// - Cannot change role of inactive user
        /// - Admin should not demote themselves (handled at application layer)
        /// </summary>
        public void ChangeRole(UserRole newRole)
        {
            if (!IsActive)
                throw new DomainException("Cannot change role of inactive user");

            Role = newRole;
        }

        /// <summary>
        /// Deactivate user account
        /// 
        /// Business rules:
        /// - Cannot deactivate already inactive user
        /// - Cannot deactivate admin users (safety check)
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive)
                throw new DomainException("User is already inactive");

            // Safety check - should also be enforced at application layer
            if (Role == UserRole.Admin)
                throw new DomainException("Cannot deactivate admin users");

            IsActive = false;
        }

        /// <summary>
        /// Activate user account
        /// 
        /// Business rule: Cannot activate already active user
        /// </summary>
        public void Activate()
        {
            if (IsActive)
                throw new DomainException("User is already active");

            IsActive = true;
        }

        /// <summary>
        /// Check if user has a specific role
        /// </summary>
        public bool HasRole(UserRole role)
        {
            return Role == role;
        }

        /// <summary>
        /// Check if user is an instructor
        /// </summary>
        public bool IsInstructor()
        {
            return Role == UserRole.Instructor;
        }

        /// <summary>
        /// Check if user is a student
        /// </summary>
        public bool IsStudent()
        {
            return Role == UserRole.Student;
        }

        /// <summary>
        /// Check if user is an admin
        /// </summary>
        public bool IsAdmin()
        {
            return Role == UserRole.Admin;
        }

        /// <summary>
        /// Check if user can login
        /// User must be active to login
        /// </summary>
        public bool CanLogin()
        {
            return IsActive;
        }
    }

    /// <summary>
    /// User role enumeration
    /// Defines the different types of users in the system
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Regular student - can enroll in courses
        /// </summary>
        Student = 0,

        /// <summary>
        /// Instructor - can create and manage courses
        /// </summary>
        Instructor = 1,

        /// <summary>
        /// Administrator - full system access
        /// </summary>
        Admin = 2
    }
}

