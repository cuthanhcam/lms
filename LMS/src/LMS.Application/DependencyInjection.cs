using FluentValidation;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace LMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();

            // Register FluentValidation
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}
