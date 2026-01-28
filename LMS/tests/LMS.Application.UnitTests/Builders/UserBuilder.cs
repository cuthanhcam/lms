using LMS.Domain.Entities;
using LMS.Domain.ValueObjects;
using System.Reflection;

namespace LMS.Application.UnitTests.Builders
{
    /// <summary>
    /// Builder pattern to create User entity for testing
    /// Helps test code more readable and maintainable
    /// </summary>
    public class UserBuilder
    {
        private Guid _id = Guid.NewGuid();
        private string _userName = "testuser";
        private string _email = "test@example.com";
        private string _passwordHash = "hashedPassword123";
        private string _role = "Student";
        private bool _isActive = true;
        private DateTime _createdAt = DateTime.UtcNow;

        public UserBuilder WithId(Guid id)
        {
            _id = id;
            return this;
        }

        public UserBuilder WithUserName(string userName)
        {
            _userName = userName;
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public UserBuilder WithPasswordHash(string passwordHash)
        {
            _passwordHash = passwordHash;
            return this;
        }

        public UserBuilder WithRole(string role)
        {
            _role = role;
            return this;
        }

        public UserBuilder AsInstructor()
        {
            _role = "Instructor";
            return this;
        }

        public UserBuilder AsAdmin()
        {
            _role = "Admin";
            return this;
        }

        public UserBuilder AsInactive()
        {
            _isActive = false;
            return this;
        }

        public User Build()
        {
            // Parse role string to enum
            var role = _role.ToLower() switch
            {
                "admin" => UserRole.Admin,
                "instructor" => UserRole.Instructor,
                _ => UserRole.Student
            };
            
            // Create Email value object
            var email = Email.Create(_email);
            
            // Use factory method to create user
            var user = User.Create(_userName, email, _passwordHash, role);
            
            // Use reflection to set Id (for testing only)
            var idProperty = typeof(User).BaseType!.GetProperty("Id");
            idProperty!.SetValue(user, _id);
            
            // Use reflection to set CreatedAt
            var createdAtField = typeof(User).GetProperty("CreatedAt", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            createdAtField!.SetValue(user, _createdAt);
            
            // Use reflection to set IsActive if needed
            if (!_isActive)
            {
                var isActiveField = typeof(User).GetProperty("IsActive", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                isActiveField!.SetValue(user, _isActive);
            }
            
            return user;
        }
    }
}
