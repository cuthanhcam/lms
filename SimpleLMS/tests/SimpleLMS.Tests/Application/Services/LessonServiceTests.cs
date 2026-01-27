using AutoMapper;
using SimpleLMS.Application.DTOs.Lessons;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Application.Services;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Enums;

namespace SimpleLMS.Tests.Application.Services;

/// <summary>
/// Unit tests for the LessonService.
/// </summary>
public class LessonServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly LessonService _lessonService;

    public LessonServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _lessonService = new LessonService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingLesson_ShouldReturnSuccess()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var lesson = new Lesson("Test Lesson", "Content", null, 1, 30, courseId);
        var lessonDto = new LessonDto { Id = lessonId, Title = "Test Lesson" };

        _unitOfWorkMock.Setup(u => u.Lessons.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);
        _mapperMock.Setup(m => m.Map<LessonDto>(lesson))
            .Returns(lessonDto);

        // Act
        var result = await _lessonService.GetByIdAsync(lessonId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be("Test Lesson");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingLesson_ShouldReturnFailure()
    {
        // Arrange
        var lessonId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Lessons.GetByIdAsync(lessonId))
            .ReturnsAsync((Lesson?)null);

        // Act
        var result = await _lessonService.GetByIdAsync(lessonId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Lesson not found");
    }

    [Fact]
    public async Task GetLessonsByCourseAsync_ShouldReturnLessons()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var lessons = new List<Lesson>
        {
            new Lesson("Lesson 1", "Content 1", null, 1, 30, courseId),
            new Lesson("Lesson 2", "Content 2", null, 2, 45, courseId)
        };

        var lessonDtos = new List<LessonDto>
        {
            new LessonDto { Title = "Lesson 1", Order = 1 },
            new LessonDto { Title = "Lesson 2", Order = 2 }
        };

        _unitOfWorkMock.Setup(u => u.Lessons.GetLessonsByCourseAsync(courseId))
            .ReturnsAsync(lessons);
        _mapperMock.Setup(m => m.Map<IEnumerable<LessonDto>>(lessons))
            .Returns(lessonDtos);

        // Act
        var result = await _lessonService.GetLessonsByCourseAsync(courseId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task CreateAsync_WithValidDataAndOwnership_ShouldCreateLesson()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var course = new Course("Test Course", "Description", 100m, instructorId);
        var createDto = new CreateLessonDto
        {
            Title = "New Lesson",
            Content = "Content",
            DurationMinutes = 30,
            Order = 1
        };

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _unitOfWorkMock.Setup(u => u.Lessons.AddAsync(It.IsAny<Lesson>()))
            .ReturnsAsync((Lesson l) => l);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<LessonDto>(It.IsAny<Lesson>()))
            .Returns(new LessonDto { Title = createDto.Title });

        // Act
        var result = await _lessonService.CreateAsync(courseId, instructorId, createDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Title.Should().Be(createDto.Title);
        _unitOfWorkMock.Verify(u => u.Lessons.AddAsync(It.IsAny<Lesson>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithoutOwnership_ShouldReturnFailure()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var otherInstructorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var course = new Course("Test Course", "Description", 100m, otherInstructorId);
        var createDto = new CreateLessonDto
        {
            Title = "New Lesson",
            Content = "Content",
            DurationMinutes = 30,
            Order = 1
        };

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _lessonService.CreateAsync(courseId, instructorId, createDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("permission");
    }

    [Fact]
    public async Task UpdateAsync_WithValidDataAndOwnership_ShouldUpdateLesson()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();
        var course = new Course("Test Course", "Description", 100m, instructorId);
        var lesson = new Lesson("Old Title", "Old Content", null, 1, 30, courseId);
        var updateDto = new UpdateLessonDto
        {
            Title = "New Title",
            Content = "New Content",
            VideoUrl = "http://video.com",
            DurationMinutes = 45
        };

        _unitOfWorkMock.Setup(u => u.Lessons.GetLessonWithCourseAsync(lessonId))
            .ReturnsAsync(lesson);
        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<LessonDto>(It.IsAny<Lesson>()))
            .Returns(new LessonDto { Title = updateDto.Title });

        // Act
        var result = await _lessonService.UpdateAsync(lessonId, instructorId, updateDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Title.Should().Be(updateDto.Title);
    }

    [Fact]
    public async Task DeleteAsync_WithValidLesson_ShouldDeleteLesson()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var lessonId = Guid.NewGuid();
        var course = new Course("Test Course", "Description", 100m, instructorId);
        var lesson = new Lesson("Title", "Content", null, 1, 30, courseId);

        _unitOfWorkMock.Setup(u => u.Lessons.GetByIdAsync(lessonId))
            .ReturnsAsync(lesson);
        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _lessonService.DeleteAsync(lessonId, instructorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
