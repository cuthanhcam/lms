using SimpleLMS.Application.DTOs.Enrollments;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Application.Services;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Enums;

namespace SimpleLMS.Tests.Application.Services;

/// <summary>
/// Unit tests for the EnrollmentService.
/// </summary>
public class EnrollmentServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly EnrollmentService _enrollmentService;

    public EnrollmentServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _enrollmentService = new EnrollmentService(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingEnrollment_ShouldReturnSuccess()
    {
        // Arrange
        var enrollmentId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollment = new Enrollment(userId, courseId);
        _unitOfWorkMock.Setup(u => u.Enrollments.GetByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);

        // Act
        var result = await _enrollmentService.GetByIdAsync(enrollmentId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetEnrollmentsByUserAsync_ShouldReturnEnrollments()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId1 = Guid.NewGuid();
        var courseId2 = Guid.NewGuid();
        var enrollments = new List<Enrollment>
        {
            new Enrollment(userId, courseId1),
            new Enrollment(userId, courseId2)
        };

        _unitOfWorkMock.Setup(u => u.Enrollments.GetEnrollmentsByUserAsync(userId))
            .ReturnsAsync(enrollments);

        // Act
        var result = await _enrollmentService.GetEnrollmentsByUserAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().HaveCount(2);
    }

    [Fact]
    public async Task EnrollAsync_WithValidData_ShouldCreateEnrollment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        var user = new User("student", "student@test.com", "hash", "Student", UserRole.Student);
        var course = new Course("Course", "Description", 100m, instructorId);
        
        // Add lesson to course so it can be published
        var lessonsField = typeof(Course).GetField("_lessons", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lessons = (List<Lesson>)lessonsField!.GetValue(course)!;
        lessons.Add(new Lesson("Lesson", "Content", null, 1, 30, courseId));
        
        course.Publish();

        var createDto = new CreateEnrollmentDto { CourseId = courseId };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _unitOfWorkMock.Setup(u => u.Enrollments.UserAlreadyEnrolledAsync(userId, courseId))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.Enrollments.AddAsync(It.IsAny<Enrollment>()))
            .ReturnsAsync((Enrollment e) => e);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _enrollmentService.EnrollAsync(userId, createDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.Enrollments.AddAsync(It.IsAny<Enrollment>()), Times.Once);
    }

    [Fact]
    public async Task EnrollAsync_WithUnpublishedCourse_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        var user = new User("student", "student@test.com", "hash", "Student", UserRole.Student);
        var course = new Course("Course", "Description", 100m, instructorId);

        var createDto = new CreateEnrollmentDto { CourseId = courseId };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync(course);

        // Act
        var result = await _enrollmentService.EnrollAsync(userId, createDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("not published");
    }

    [Fact]
    public async Task EnrollAsync_WithExistingEnrollment_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var instructorId = Guid.NewGuid();
        var user = new User("student", "student@test.com", "hash", "Student", UserRole.Student);
        var course = new Course("Course", "Description", 100m, instructorId);
        
        // Add lesson to course so it can be published
        var lessonsField = typeof(Course).GetField("_lessons", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var lessons = (List<Lesson>)lessonsField!.GetValue(course)!;
        lessons.Add(new Lesson("Lesson", "Content", null, 1, 30, courseId));
        
        course.Publish();
        var existingEnrollment = new Enrollment(userId, courseId);

        var createDto = new CreateEnrollmentDto { CourseId = courseId };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.Courses.GetByIdAsync(courseId))
            .ReturnsAsync(course);
        _unitOfWorkMock.Setup(u => u.Enrollments.UserAlreadyEnrolledAsync(userId, courseId))
            .ReturnsAsync(true);

        // Act
        var result = await _enrollmentService.EnrollAsync(userId, createDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("already enrolled");
    }

    [Fact]
    public async Task UpdateProgressAsync_WithValidData_ShouldUpdateProgress()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollment = new Enrollment(userId, courseId);
        var updateDto = new UpdateProgressDto { ProgressPercentage = 50 };

        _unitOfWorkMock.Setup(u => u.Enrollments.GetByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _enrollmentService.UpdateProgressAsync(enrollmentId, userId, updateDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateProgressAsync_WithoutOwnership_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var otherUserId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollment = new Enrollment(otherUserId, courseId);
        var updateDto = new UpdateProgressDto { ProgressPercentage = 50 };

        _unitOfWorkMock.Setup(u => u.Enrollments.GetByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);

        // Act
        var result = await _enrollmentService.UpdateProgressAsync(enrollmentId, userId, updateDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("permission");
    }

    [Fact]
    public async Task CompleteAsync_WithValidEnrollment_ShouldCompleteEnrollment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollment = new Enrollment(userId, courseId);

        _unitOfWorkMock.Setup(u => u.Enrollments.GetByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _enrollmentService.CompleteAsync(enrollmentId, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CancelAsync_WithValidEnrollment_ShouldCancelEnrollment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var enrollment = new Enrollment(userId, courseId);

        _unitOfWorkMock.Setup(u => u.Enrollments.GetByIdAsync(enrollmentId))
            .ReturnsAsync(enrollment);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _enrollmentService.CancelAsync(enrollmentId, userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
