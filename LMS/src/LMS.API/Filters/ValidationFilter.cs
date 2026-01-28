using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LMS.API.Filters
{
    /// <summary>
    /// Action filter to automatically validate request DTOs using FluentValidation
    /// This replaces the deprecated AddFluentValidation() method in FluentValidation 11.x+
    /// </summary>
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Iterate through all action parameters
            foreach (var parameter in context.ActionDescriptor.Parameters)
            {
                // Get the parameter value from ActionArguments
                if (context.ActionArguments.TryGetValue(parameter.Name, out var argumentValue) 
                    && argumentValue != null)
                {
                    var argumentType = argumentValue.GetType();

                    // Get the validator type for this argument type
                    var validatorType = typeof(IValidator<>).MakeGenericType(argumentType);

                    // Try to resolve validator from DI container
                    var validator = _serviceProvider.GetService(validatorType) as IValidator;

                    if (validator != null)
                    {
                        // Create validation context
                        var validationContext = new ValidationContext<object>(argumentValue);

                        // Validate the object
                        var validationResult = await validator.ValidateAsync(validationContext);

                        if (!validationResult.IsValid)
                        {
                            // Add validation errors to ModelState
                            foreach (var error in validationResult.Errors)
                            {
                                context.ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                            }

                            // Return 400 Bad Request with validation errors
                            context.Result = new BadRequestObjectResult(new
                            {
                                status = 400,
                                message = "Validation failed",
                                errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                            });

                            return;
                        }
                    }
                }
            }

            // If validation passed, continue to the action
            await next();
        }
    }
}
