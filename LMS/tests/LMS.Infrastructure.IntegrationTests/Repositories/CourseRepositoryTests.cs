using FluentAssertions;
using LMS.Domain.Entities;
using LMS.Domain.ValueObjects;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.IntegrationTests.Repositories;

public class CourseRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly CourseRepository _repository;

    public CourseRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new AppDbContext(options);
        _dbContext.Database.EnsureCreated();
        _repository = new CourseRepository(_dbContext);
    }

    [Fact]
    public async Task GetAllAsync_ShouldExcludeSoftDeletedCourses()
    {
        var creator = User.Create("instructor", Email.Create("instructor@example.com"), "hashed", UserRole.Instructor);
        await _dbContext.Users.AddAsync(creator);

        var activeCourse = Course.Create("Active", "Course", Money.Create(10m), creator.Id);
        var deletedCourse = Course.Create("Deleted", "Course", Money.Create(10m), creator.Id);
        deletedCourse.Delete();

        await _dbContext.Courses.AddRangeAsync(activeCourse, deletedCourse);
        await _dbContext.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        result.Should().ContainSingle(c => c.Title == "Active");
        result.Should().NotContain(c => c.Title == "Deleted");
    }

    [Fact]
    public async Task CanPublishAsync_ShouldReturnFalse_WhenOnlyLessonIsSoftDeleted()
    {
        var creator = User.Create("instructor", Email.Create("owner@example.com"), "hashed", UserRole.Instructor);
        await _dbContext.Users.AddAsync(creator);

        var course = Course.Create("Course", "Desc", Money.Create(20m), creator.Id);
        var lesson = Lesson.Create("Lesson 1", "Content", 1);
        course.AddLesson(lesson);
        lesson.Delete();

        await _dbContext.Courses.AddAsync(course);
        await _dbContext.SaveChangesAsync();

        var canPublish = await _repository.CanPublishAsync(course.Id);

        canPublish.Should().BeFalse();
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }
}
