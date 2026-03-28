using FluentAssertions;
using LMS.Domain.Entities;
using LMS.Domain.Exceptions;

namespace LMS.Application.UnitTests.Domain.Entities;

public class EnrollmentDomainTests
{
    [Fact]
    public void UpdateProgress_ToOneHundred_ShouldAutoCompleteEnrollment()
    {
        var enrollment = Enrollment.Create(Guid.NewGuid(), Guid.NewGuid());

        enrollment.UpdateProgress(100);

        enrollment.Status.Should().Be(EnrollmentStatus.Completed);
        enrollment.ProgressPercentage.Should().Be(100);
        enrollment.CompletedAt.Should().NotBeNull();
    }

    [Fact]
    public void Cancel_ThenUpdateProgress_ShouldThrowDomainException()
    {
        var enrollment = Enrollment.Create(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Cancel();

        Action act = () => enrollment.UpdateProgress(10);

        act.Should().Throw<DomainException>()
            .WithMessage("*cancelled enrollment*");
    }

    [Fact]
    public void Complete_WhenAlreadyCompleted_ShouldThrowDomainException()
    {
        var enrollment = Enrollment.Create(Guid.NewGuid(), Guid.NewGuid());
        enrollment.Complete();

        Action act = () => enrollment.Complete();

        act.Should().Throw<DomainException>()
            .WithMessage("*already completed*");
    }
}
