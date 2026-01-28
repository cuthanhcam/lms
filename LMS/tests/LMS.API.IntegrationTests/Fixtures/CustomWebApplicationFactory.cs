using LMS.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LMS.API.IntegrationTests.Fixtures
{
    /// <summary>
    /// Custom WebApplicationFactory to setup test environment
    /// - Uses InMemory database instead of SQL Server
    /// - Automatically seeds data for tests
    /// </summary>
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing DbContext registration
                services.RemoveAll(typeof(DbContextOptions<AppDbContext>));

                // Add InMemory database for testing
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDatabase");
                });

                // Build service provider
                var serviceProvider = services.BuildServiceProvider();

                // Create scope v� seed database
                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                // Ensure database is created
                db.Database.EnsureCreated();

                // Seed test data n?u c?n
                SeedTestData(db);
            });
        }

        /// <summary>
        /// Seed initial test data v�o database
        /// </summary>
        private void SeedTestData(AppDbContext context)
        {
            // Clear existing data
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // C� th? th�m test data ? ?�y n?u c?n
            // V� d?: T?o 1 admin user, 1 instructor, 1 student
        }
    }
}
