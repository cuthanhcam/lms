using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Enums;
using SimpleLMS.Domain.Exceptions;

namespace SimpleLMS.Tests.Domain.Entities;

/// <summary>
/// Unit tests for the Enrollment entity.
/// </summary>
public class EnrollmentTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateEnrollment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();

        // Act
        var enrollment = new Enrollment(userId, courseId);

        // Assert
        enrollment.UserId.Should().Be(userId);
        enrollment.CourseId.Should().Be(courseId);
        enrollment.Status.Should().Be(EnrollmentStatus.Active);
        enrollment.ProgressPercentage.Should().Be(0);
        enrollment.EnrolledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Constructor_WithEmptyUserId_ShouldThrowException()
    {
        // Arrange & Act
        Action act = () => new Enrollment(Guid.Empty, Guid.NewGuid());

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*UserId*required*");
    }

    [Fact]
    public void Constructor_WithEmptyCourseId_ShouldThrowException()
    {
        // Arrange & Act
        Action act = () => new Enrollment(Guid.NewGuid(), Guid.Empty);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*CourseId*required*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(50)]
    [InlineData(100)]
    public void UpdateProgress_WithValidPercentage_ShouldUpdateProgress(int percentage)
    {
        // Arrange
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());

        // Act
        enrollment.UpdateProgress(percentage);

        // Assert
        enrollment.ProgressPercentage.Should().Be(percentage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void UpdateProgress_WithInvalidPercentage_ShouldThrowException(int invalidPercentage)
    {
        // Arrange
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());

        // Act
        Action act = () => enrollment.UpdateProgress(invalidPercentage);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Progress*between 0 and 100*");
    }

    [Fact]
    public void UpdateProgress_WhenCompleted_ShouldThrowException()
    {
        // Arrange
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Complete();

        // Act
        Action act = () => enrollment.UpdateProgress(50);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Cannot update*completed*");
    }

    [Fact]
    public void UpdateProgress_WhenCancelled_ShouldThrowException()
    {
        // Arrange
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Cancel();

        // Act
        Action act = () => enrollment.UpdateProgress(50);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Cannot update*cancelled*");
    }

    [Fact]
    public void Complete_WhenActive_ShouldCompleteEnrollment()
    {
        // Arrange
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());

        // Act
        enrollment.Complete();

        // Assert
        enrollment.Status.Should().Be(EnrollmentStatus.Completed);
        enrollment.CompletedAt.Should().NotBeNull();
        enrollment.CompletedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        enrollment.ProgressPercentage.Should().Be(100);
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ShouldThrowException()
    {
        // Arrange
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Complete();

        // Act
        Action act = () => enrollment.Complete();

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*active*completed*");
    }

    [Fact]
    public void Complete_WhenCancelled_ShouldThrowException()
    {
        // Arrange
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Cancel();

        // Act
        Action act = () => enrollment.Complete();

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*active*completed*");
    }

    [Fact]
    public void Cancel_WhenActive_ShouldCancelEnrollment()
    {
        // Arrange
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());

        // Act
        enrollment.Cancel();

        // Assert
        enrollment.Status.Should().Be(EnrollmentStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenCompleted_ShouldThrowException()
    {
        // Arrange
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Complete();

        // Act
        Action act = () => enrollment.Cancel();

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*active*cancelled*");
    }

    [Fact]
    public void Cancel_WhenAlreadyCancelled_ShouldThrowException()
    {
        // Arrange
        var enrollment = new Enrollment(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Cancel();

        // Act
        Action act = () => enrollment.Cancel();

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*active*cancelled*");
    }
}
