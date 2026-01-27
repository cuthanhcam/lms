using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Exceptions;

namespace SimpleLMS.Tests.Domain.Entities;

/// <summary>
/// Unit tests for the Course entity.
/// </summary>
public class CourseTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateCourse()
    {
        // Arrange
        var title = "Test Course";
        var description = "Test Description";
        var price = 100m;
        var instructorId = Guid.NewGuid();

        // Act
        var course = new Course(title, description, price, instructorId);

        // Assert
        course.Title.Should().Be(title);
        course.Description.Should().Be(description);
        course.Price.Should().Be(price);
        course.InstructorId.Should().Be(instructorId);
        course.IsPublished.Should().BeFalse();
        course.IsDeleted.Should().BeFalse();
        course.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidTitle_ShouldThrowException(string invalidTitle)
    {
        // Arrange & Act
        Action act = () => new Course(invalidTitle, "Description", 100m, Guid.NewGuid());

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Title*required*");
    }

    [Fact]
    public void Constructor_WithNegativePrice_ShouldThrowException()
    {
        // Arrange & Act
        Action act = () => new Course("Title", "Description", -10m, Guid.NewGuid());

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Price*negative*");
    }

    [Fact]
    public void Constructor_WithEmptyInstructorId_ShouldThrowException()
    {
        // Arrange & Act
        Action act = () => new Course("Title", "Description", 100m, Guid.Empty);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*InstructorId*required*");
    }

    [Fact]
    public void UpdateInfo_WithValidData_ShouldUpdateCourse()
    {
        // Arrange
        var course = new Course("Old Title", "Old Description", 100m, Guid.NewGuid());
        var newTitle = "New Title";
        var newDescription = "New Description";
        var newPrice = 200m;

        // Act
        course.UpdateInfo(newTitle, newDescription, newPrice);

        // Assert
        course.Title.Should().Be(newTitle);
        course.Description.Should().Be(newDescription);
        course.Price.Should().Be(newPrice);
        course.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Publish_WhenNotPublished_ShouldPublishCourse()
    {
        // Arrange
        var course = new Course("Title", "Description", 100m, Guid.NewGuid());

        // Act
        course.Publish();

        // Assert
        course.IsPublished.Should().BeTrue();
        course.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_ShouldThrowException()
    {
        // Arrange
        var course = new Course("Title", "Description", 100m, Guid.NewGuid());
        course.Publish();

        // Act
        Action act = () => course.Publish();

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*already published*");
    }

    [Fact]
    public void Unpublish_WhenPublished_ShouldUnpublishCourse()
    {
        // Arrange
        var course = new Course("Title", "Description", 100m, Guid.NewGuid());
        course.Publish();

        // Act
        course.Unpublish();

        // Assert
        course.IsPublished.Should().BeFalse();
        course.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Unpublish_WhenNotPublished_ShouldThrowException()
    {
        // Arrange
        var course = new Course("Title", "Description", 100m, Guid.NewGuid());

        // Act
        Action act = () => course.Unpublish();

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*not published*");
    }

    [Fact]
    public void Delete_ShouldMarkAsDeleted()
    {
        // Arrange
        var course = new Course("Title", "Description", 100m, Guid.NewGuid());

        // Act
        course.Delete();

        // Assert
        course.IsDeleted.Should().BeTrue();
        course.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Delete_WhenPublished_ShouldThrowException()
    {
        // Arrange
        var course = new Course("Title", "Description", 100m, Guid.NewGuid());
        course.Publish();

        // Act
        Action act = () => course.Delete();

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Cannot delete*published*");
    }
}
