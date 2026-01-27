using Microsoft.Extensions.DependencyInjection;
using SimpleLMS.Application.Interfaces.Services;
using SimpleLMS.Application.Mappings;
using SimpleLMS.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Application
{
    /// <summary>
    /// Application layer dependency injection extensions.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // AutoMapper
            services.AddAutoMapper(config =>
            {
                config.AddProfile<MappingProfile>();
            });

            // Services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<ILessonService, LessonService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();

            return services;
        }
    }
}
