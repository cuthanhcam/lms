using LMS.Application.Interfaces.Identity;
using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Common;
using LMS.Infrastructure.Data;
using LMS.Infrastructure.Events;
using LMS.Infrastructure.Identity;
using LMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LMS.Infrastructure
{
    /// <summary>
    /// Infrastructure layer dependency injection configuration
    /// Registers all infrastructure services, repositories, and event dispatching
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
        {
            // ==================== DATABASE ====================
            
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            // ==================== REPOSITORIES ====================
            
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<ILessonRepository, LessonRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();

            // ==================== IDENTITY SERVICES ====================
            
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();

            // ==================== DOMAIN EVENTS ====================
            
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

            // TODO: Register domain event handlers here
            // Example:
            // services.AddScoped<IDomainEventHandler<CoursePublishedEvent>, CoursePublishedEventHandler>();
            // services.AddScoped<IDomainEventHandler<EnrollmentCreatedEvent>, EnrollmentCreatedEventHandler>();

            return services;
        }
    }
}
