using Microsoft.EntityFrameworkCore;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Enums;
using SimpleLMS.Infrastructure.Persistence;
using SimpleLMS.Infrastructure.Repositories;

namespace SimpleLMS.Tests.Infrastructure.Repositories;

/// <summary>
/// Unit tests for the CourseRepository.
/// </summary>
public class CourseRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly CourseRepository _repository;

    public CourseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new CourseRepository(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public async Task GetPublishedCoursesAsync_ShouldReturnOnlyPublishedCourses()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var instructor = new User("instructor", "inst@test.com", "hash", "Instructor", UserRole.Instructor);
        _context.Users.Add(instructor);
        await _context.SaveChangesAsync();

        var publishedCourse = new Course("Published", "Desc", 100m, instructor.Id);
        var unpublishedCourse = new Course("Unpublished", "Desc", 100m, instructor.Id);

        _context.Courses.AddRange(publishedCourse, unpublishedCourse);
        await _context.SaveChangesAsync();

        // Add lesson to published course so it can be published
        var lesson = new Lesson("Lesson", "Content", null, 1, 30, publishedCourse.Id);
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();

        // Now publish the course
        publishedCourse.Publish();
        _context.Courses.Update(publishedCourse);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPublishedCoursesAsync();

        // Assert
        result.Should().HaveCount(1);
        result.First().IsPublished.Should().BeTrue();
        result.First().Title.Should().Be("Published");
    }

    [Fact]
    public async Task GetPublishedCoursesAsync_ShouldIncludeLessons()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var instructor = new User("instructor", "inst@test.com", "hash", "Instructor", UserRole.Instructor);
        _context.Users.Add(instructor);
        await _context.SaveChangesAsync();

        var course = new Course("Course", "Desc", 100m, instructor.Id);
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        var lesson = new Lesson("Lesson", "Content", null, 1, 30, course.Id);
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();

        // Publish after adding lesson
        course.Publish();
        _context.Courses.Update(course);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPublishedCoursesAsync();

        // Assert
        var retrievedCourse = result.First();
        retrievedCourse.Lessons.Should().HaveCount(1);
        retrievedCourse.Lessons.First().Title.Should().Be("Lesson");
    }

    [Fact]
    public async Task GetCoursesByInstructorAsync_ShouldReturnOnlyInstructorCourses()
    {
        // Arrange
        var instructor1Id = Guid.NewGuid();
        var instructor2Id = Guid.NewGuid();
        
        var instructor1 = new User("inst1", "inst1@test.com", "hash", "Instructor 1", UserRole.Instructor);
        var instructor2 = new User("inst2", "inst2@test.com", "hash", "Instructor 2", UserRole.Instructor);
        _context.Users.AddRange(instructor1, instructor2);
        await _context.SaveChangesAsync();

        var course1 = new Course("Course 1", "Desc", 100m, instructor1.Id);
        var course2 = new Course("Course 2", "Desc", 100m, instructor2.Id);
        _context.Courses.AddRange(course1, course2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCoursesByInstructorAsync(instructor1.Id);

        // Assert
        result.Should().HaveCount(1);
        result.First().InstructorId.Should().Be(instructor1.Id);
    }

    [Fact]
    public async Task GetCourseWithLessonsAsync_ShouldIncludeLessonsOrderedByOrder()
    {
        // Arrange
        var instructorId = Guid.NewGuid();
        var instructor = new User("instructor", "inst@test.com", "hash", "Instructor", UserRole.Instructor);
        _context.Users.Add(instructor);
        await _context.SaveChangesAsync();

        var course = new Course("Course", "Desc", 100m, instructor.Id);
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        var lesson1 = new Lesson("Lesson 1", "Content", null, 2, 30, course.Id);
        var lesson2 = new Lesson("Lesson 2", "Content", null, 1, 30, course.Id);
        var lesson3 = new Lesson("Lesson 3", "Content", null, 3, 30, course.Id);
        _context.Lessons.AddRange(lesson1, lesson2, lesson3);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetCourseWithLessonsAsync(course.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Lessons.Should().HaveCount(3);
        result.Lessons.First().Order.Should().Be(1);
        result.Lessons.Last().Order.Should().Be(3);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeLessonsAndInstructor()
    {
        // Arrange
        var instructor = new User("instructor", "inst@test.com", "hash", "Instructor", UserRole.Instructor);
        _context.Users.Add(instructor);
        await _context.SaveChangesAsync();

        var course = new Course("Course", "Desc", 100m, instructor.Id);
        _context.Courses.Add(course);
        await _context.SaveChangesAsync();

        var lesson = new Lesson("Lesson", "Content", null, 1, 30, course.Id);
        _context.Lessons.Add(lesson);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(course.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Instructor.Should().NotBeNull();
        result.Instructor!.FullName.Should().Be("Instructor");
        result.Lessons.Should().HaveCount(1);
    }
}
