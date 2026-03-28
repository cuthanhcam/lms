using FluentAssertions;
using LMS.Application.DTOs.Auth;
using LMS.Application.DTOs.Courses;
using LMS.Application.Validators;

namespace LMS.Application.UnitTests.Validators;

public class RegisterAndCourseValidatorTests
{
    [Fact]
    public void RegisterValidator_WithValidRequest_ShouldPass()
    {
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            UserName = "validuser",
            Email = "valid@example.com",
            Password = "Valid123",
            Role = "Student"
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void RegisterValidator_WithWeakPassword_ShouldFail()
    {
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            UserName = "user",
            Email = "valid@example.com",
            Password = "weak",
            Role = "Student"
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Password");
    }

    [Fact]
    public void RegisterValidator_WithInvalidRole_ShouldFail()
    {
        var validator = new RegisterRequestValidator();
        var request = new RegisterRequest
        {
            UserName = "user",
            Email = "valid@example.com",
            Password = "Valid123",
            Role = "SuperAdmin"
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Role");
    }

    [Fact]
    public void CreateCourseValidator_WithNegativePrice_ShouldFail()
    {
        var validator = new CreateCourseRequestValidator();
        var request = new CreateCourseRequest
        {
            Title = "Test course",
            Description = "Description",
            Price = -1
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Price");
    }

    [Fact]
    public void CreateCourseValidator_WithTitleTooLong_ShouldFail()
    {
        var validator = new CreateCourseRequestValidator();
        var request = new CreateCourseRequest
        {
            Title = new string('A', 201),
            Description = "Description",
            Price = 10
        };

        var result = validator.Validate(request);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(x => x.PropertyName == "Title");
    }
}
