using FluentValidation;
using LMS.Application.DTOs.Lessons;

namespace LMS.Application.Validators
{
    public class UpdateLessonRequestValidator : AbstractValidator<UpdateLessonRequest>
    {
        public UpdateLessonRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title must not exceed 200 characters");

            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("Content is required");

            RuleFor(x => x.Order)
                .GreaterThan(0).WithMessage("Order must be greater than 0");
        }
    }
}
