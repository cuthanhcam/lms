using SimpleLMS.Domain.Common;
using SimpleLMS.Domain.Enums;
using SimpleLMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FullName { get; private set; }
        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        private readonly List<Course> _createdCourses = new();
        public IReadOnlyCollection<Course> CreatedCourses => _createdCourses.AsReadOnly();

        private readonly List<Enrollment> _enrollments = new();
        public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

        // Constructor for EF Core
        private User() { }

        public User(string username, string email, string passwordHash, string fullName, UserRole role)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new BusinessRuleViolationException("Username cannot be empty.", nameof(username));
            if (string.IsNullOrWhiteSpace(email))
                throw new BusinessRuleViolationException("Email cannot be empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new BusinessRuleViolationException("PasswordHash cannot be empty.", nameof(passwordHash));
            if (string.IsNullOrWhiteSpace(fullName))
                throw new BusinessRuleViolationException("FullName cannot be empty.", nameof(fullName));

            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            FullName = fullName;
            Role = role;
            IsActive = true;
        }

        // Update user information
        public void UpdateInfo(string email, string fullName)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new BusinessRuleViolationException("Email cannot be empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(fullName))
                throw new BusinessRuleViolationException("FullName cannot be empty.", nameof(fullName));
            Email = email;
            FullName = fullName;
            MarkAsModified();
        }

        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new BusinessRuleViolationException("New password hash cannot be empty.", nameof(newPasswordHash));
            
            PasswordHash = newPasswordHash;
            MarkAsModified();
        }

        // Deactivate user
        public void Deactivate()
        {
            IsActive = false;
            MarkAsModified();
        }

        // Activate user
        public void Activate()
        {
            IsActive = true;
            MarkAsModified();
        }

        // Change user role
        public void ChangeRole(UserRole newRole)
        {
            Role = newRole;
            MarkAsModified();
        }
    }
}
