using FluentAssertions;
using LMS.Domain.Exceptions;
using LMS.Domain.ValueObjects;

namespace LMS.Application.UnitTests.Domain.ValueObjects;

public class EmailTests
{
    [Fact]
    public void Create_WithInvalidFormat_ShouldThrowDomainException()
    {
        Action act = () => Email.Create("invalid-email");

        act.Should().Throw<DomainException>()
            .WithMessage("*not a valid email*");
    }

    [Fact]
    public void Create_ShouldTrimAndLowercaseEmail()
    {
        var email = Email.Create("  USER@Example.COM  ");

        email.Value.Should().Be("user@example.com");
    }

    [Fact]
    public void Equality_WithSameValueIgnoringCaseNormalization_ShouldBeTrue()
    {
        var left = Email.Create("USER@example.com");
        var right = Email.Create("user@example.com");

        (left == right).Should().BeTrue();
        left.Equals(right).Should().BeTrue();
    }
}
