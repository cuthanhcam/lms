using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Enums;
using SimpleLMS.Domain.Exceptions;

namespace SimpleLMS.Tests.Domain.Entities;

/// <summary>
/// Unit tests for the User entity.
/// </summary>
public class UserTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var username = "johndoe";
        var email = "john@example.com";
        var passwordHash = "hashedpassword123";
        var fullName = "John Doe";
        var role = UserRole.Student;

        // Act
        var user = new User(username, email, passwordHash, fullName, role);

        // Assert
        user.Username.Should().Be(username);
        user.Email.Should().Be(email);
        user.PasswordHash.Should().Be(passwordHash);
        user.FullName.Should().Be(fullName);
        user.Role.Should().Be(role);
        user.IsDeleted.Should().BeFalse();
        user.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidUsername_ShouldThrowException(string invalidUsername)
    {
        // Arrange & Act
        Action act = () => new User(invalidUsername, "email@test.com", "hash", "Name", UserRole.Student);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Username*required*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidEmail_ShouldThrowException(string invalidEmail)
    {
        // Arrange & Act
        Action act = () => new User("username", invalidEmail, "hash", "Name", UserRole.Student);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Email*required*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidPasswordHash_ShouldThrowException(string invalidHash)
    {
        // Arrange & Act
        Action act = () => new User("username", "email@test.com", invalidHash, "Name", UserRole.Student);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Password*required*");
    }

    [Fact]
    public void UpdateInfo_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var user = new User("olduser", "old@test.com", "hash", "Old Name", UserRole.Student);
        var newFullName = "New Name";
        var newEmail = "new@test.com";

        // Act
        user.UpdateInfo(newFullName, newEmail);

        // Assert
        user.FullName.Should().Be(newFullName);
        user.Email.Should().Be(newEmail);
        user.UpdatedAt.Should().NotBeNull();
        user.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Delete_ShouldMarkAsDeleted()
    {
        // Arrange
        var user = new User("username", "email@test.com", "hash", "Name", UserRole.Student);

        // Act
        user.Delete();

        // Assert
        user.IsDeleted.Should().BeTrue();
        user.UpdatedAt.Should().NotBeNull();
    }
}
