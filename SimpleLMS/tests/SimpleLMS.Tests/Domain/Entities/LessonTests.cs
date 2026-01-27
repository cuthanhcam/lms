using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Exceptions;

namespace SimpleLMS.Tests.Domain.Entities;

/// <summary>
/// Unit tests for the Lesson entity.
/// </summary>
public class LessonTests
{
    [Fact]
    public void Constructor_WithValidData_ShouldCreateLesson()
    {
        // Arrange
        var title = "Test Lesson";
        var content = "Test Content";
        var videoUrl = "http://video.com";
        var order = 1;
        var durationMinutes = 30;
        var courseId = Guid.NewGuid();

        // Act
        var lesson = new Lesson(title, content, videoUrl, order, durationMinutes, courseId);

        // Assert
        lesson.Title.Should().Be(title);
        lesson.Content.Should().Be(content);
        lesson.VideoUrl.Should().Be(videoUrl);
        lesson.Order.Should().Be(order);
        lesson.DurationMinutes.Should().Be(durationMinutes);
        lesson.CourseId.Should().Be(courseId);
        lesson.IsDeleted.Should().BeFalse();
        lesson.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidTitle_ShouldThrowException(string invalidTitle)
    {
        // Arrange & Act
        Action act = () => new Lesson(invalidTitle, "Content", null, 1, 30, Guid.NewGuid());

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Title*");
    }

    [Theory]
    [InlineData(-1)]
    public void Constructor_WithInvalidDuration_ShouldThrowException(int invalidDuration)
    {
        // Arrange & Act
        Action act = () => new Lesson("Title", "Content", null, 1, invalidDuration, Guid.NewGuid());

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Duration*");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Constructor_WithInvalidOrder_ShouldThrowException(int invalidOrder)
    {
        // Arrange & Act
        Action act = () => new Lesson("Title", "Content", null, invalidOrder, 30, Guid.NewGuid());

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*Order*");
    }

    [Fact]
    public void Constructor_WithEmptyCourseId_ShouldThrowException()
    {
        // Arrange & Act
        Action act = () => new Lesson("Title", "Content", null, 1, 30, Guid.Empty);

        // Assert
        act.Should().Throw<BusinessRuleViolationException>()
            .WithMessage("*CourseId*");
    }

    [Fact]
    public void UpdateInfo_WithValidData_ShouldUpdateLesson()
    {
        // Arrange
        var lesson = new Lesson("Old Title", "Old Content", null, 1, 30, Guid.NewGuid());
        var newTitle = "New Title";
        var newContent = "New Content";
        var newVideoUrl = "http://newvideo.com";
        var newDuration = 45;

        // Act
        lesson.UpdateInfo(newTitle, newContent, newVideoUrl, newDuration);

        // Assert
        lesson.Title.Should().Be(newTitle);
        lesson.Content.Should().Be(newContent);
        lesson.VideoUrl.Should().Be(newVideoUrl);
        lesson.DurationMinutes.Should().Be(newDuration);
        lesson.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Delete_ShouldMarkAsDeleted()
    {
        // Arrange
        var lesson = new Lesson("Title", "Content", null, 1, 30, Guid.NewGuid());

        // Act
        lesson.Delete();

        // Assert
        lesson.IsDeleted.Should().BeTrue();
        lesson.UpdatedAt.Should().NotBeNull();
    }
}

