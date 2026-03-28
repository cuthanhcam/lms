using FluentAssertions;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;
using LMS.Domain.ValueObjects;

namespace LMS.Domain.UnitTests.Entities;

public class CourseDomainTests
{
    [Fact]
    public void Publish_WithoutLessons_ShouldThrowDomainException()
    {
        var course = Course.Create("Course", "Desc", Money.Create(10m), Guid.NewGuid());

        Action act = () => course.Publish();

        act.Should().Throw<DomainException>()
            .WithMessage("*without lessons*");
    }

    [Fact]
    public void Publish_WithLesson_ShouldSetIsPublishedTrue()
    {
        var course = Course.Create("Course", "Desc", Money.Create(10m), Guid.NewGuid());
        var lesson = Lesson.Create("Lesson 1", "Content", 1);
        course.AddLesson(lesson);

        course.Publish();

        course.IsPublished.Should().BeTrue();
    }

    [Fact]
    public void Delete_WhenPublished_ShouldThrowDomainException()
    {
        var course = Course.Create("Course", "Desc", Money.Create(10m), Guid.NewGuid());
        course.AddLesson(Lesson.Create("Lesson 1", "Content", 1));
        course.Publish();

        Action act = () => course.Delete();

        act.Should().Throw<DomainException>()
            .WithMessage("*Unpublish*");
    }

    [Fact]
    public void RemoveLesson_WhenPublished_ShouldThrowDomainException()
    {
        var course = Course.Create("Course", "Desc", Money.Create(10m), Guid.NewGuid());
        var lesson = Lesson.Create("Lesson 1", "Content", 1);
        course.AddLesson(lesson);
        course.Publish();

        Action act = () => course.RemoveLesson(lesson.Id);

        act.Should().Throw<DomainException>()
            .WithMessage("*published course*");
    }
}
