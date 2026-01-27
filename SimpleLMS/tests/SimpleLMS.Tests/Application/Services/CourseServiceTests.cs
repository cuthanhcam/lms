using AutoMapper;
using SimpleLMS.Application.Common;
using SimpleLMS.Application.DTOs.Courses;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Application.Services;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Enums;

namespace SimpleLMS.Tests.Application.Services;

/// <summary>
/// Unit tests for the CourseService.
/// </summary>
public class CourseServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CourseService _courseService;

    public CourseServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _courseService = new CourseService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingCourse_ShouldReturnSuccess()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        var course = new Course("Test Course", "Description", 100m, instructorId);
        var courseDto = new CourseDto { Id = courseId, Title = "Test Course" };

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _mapperMock.Setup(m => m.Map<CourseDto>(course))
            .Returns(courseDto);

        // Act
        var result = await _courseService.GetByIdAsync(courseId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Id.Should().Be(courseId);
        result.Data.Title.Should().Be("Test Course");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingCourse_ShouldReturnFailure()
    {
        // Arrange
        var courseId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync((Course?)null);

        // Act
        var result = await _courseService.GetByIdAsync(courseId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Course not found");
    }

    [Fact]
    public async Task GetPublishedCoursesAsync_ShouldReturnPublishedCourses()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var courses = new List<Course>
        {
            new Course("Course 1", "Desc 1", 100m, instructorId),
            new Course("Course 2", "Desc 2", 200m, instructorId)
        };
        courses[0].Publish();
        courses[1].Publish();

        var courseDtos = new List<CourseDto>
        {
            new CourseDto { Id = Guid.NewGuid(), Title = "Course 1", IsPublished = true },
            new CourseDto { Id = Guid.NewGuid(), Title = "Course 2", IsPublished = true }
        };

        _unitOfWorkMock.Setup(u => u.Courses.GetPublishedCoursesAsync())
            .ReturnsAsync(courses);
        _mapperMock.Setup(m => m.Map<IEnumerable<CourseDto>>(courses))
            .Returns(courseDtos);

        // Act
        var result = await _courseService.GetPublishedCoursesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().OnlyContain(c => c.IsPublished);
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateCourse()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var instructor = new User("instructor", "instructor@test.com", "hash", "Instructor", UserRole.Instructor);
        var createDto = new CreateCourseDto
        {
            Title = "New Course",
            Description = "Description",
            Price = 100m
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(instructorId))
            .ReturnsAsync(instructor);
        _unitOfWorkMock.Setup(u => u.Courses.AddAsync(It.IsAny<Course>()))
            .ReturnsAsync((Course c) => c);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<CourseDto>(It.IsAny<Course>()))
            .Returns(new CourseDto { Title = createDto.Title });

        // Act
        var result = await _courseService.CreateAsync(instructorId, createDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Title.Should().Be(createDto.Title);
        _unitOfWorkMock.Verify(u => u.Courses.AddAsync(It.IsAny<Course>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithNonExistingInstructor_ShouldReturnFailure()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var createDto = new CreateCourseDto
        {
            Title = "New Course",
            Description = "Description",
            Price = 100m
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(instructorId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _courseService.CreateAsync(instructorId, createDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("Instructor not found");
    }

    [Fact]
    public async Task UpdateAsync_WithValidDataAndOwnership_ShouldUpdateCourse()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var course = new Course("Old Title", "Old Desc", 100m, instructorId);
        var updateDto = new UpdateCourseDto
        {
            Title = "New Title",
            Description = "New Desc",
            Price = 200m
        };

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<CourseDto>(It.IsAny<Course>()))
            .Returns(new CourseDto { Title = updateDto.Title });

        // Act
        var result = await _courseService.UpdateAsync(courseId, instructorId, updateDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Title.Should().Be(updateDto.Title);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WithoutOwnership_ShouldReturnFailure()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var otherInstructorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var course = new Course("Title", "Desc", 100m, otherInstructorId);
        var updateDto = new UpdateCourseDto
        {
            Title = "New Title",
            Description = "New Desc",
            Price = 200m
        };

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _courseService.UpdateAsync(courseId, instructorId, updateDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("permission");
    }

    [Fact]
    public async Task PublishAsync_WithValidCourse_ShouldPublishCourse()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var course = new Course("Title", "Desc", 100m, instructorId);
        
        // Use reflection to add a lesson to the course's internal collection
        // so it can be published (business rule: course must have at least 1 lesson)
        var lessonsField = typeof(Course).GetField("_lessons", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lessons = (List<Lesson>)lessonsField!.GetValue(course)!;
        lessons.Add(new Lesson("Lesson", "Content", null, 1, 30, courseId));

        _unitOfWorkMock.Setup(u => u.Courses.GetCourseWithLessonsAsync(courseId))
            .ReturnsAsync(course);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _courseService.PublishAsync(courseId, instructorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithUnpublishedCourse_ShouldDeleteCourse()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var course = new Course("Title", "Desc", 100m, instructorId);

        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _courseService.DeleteAsync(courseId, instructorId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
