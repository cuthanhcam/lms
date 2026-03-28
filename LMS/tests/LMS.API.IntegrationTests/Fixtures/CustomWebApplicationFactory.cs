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

                // Create scope and seed database
                using var scope = serviceProvider.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();

                // Ensure database is created
                db.Database.EnsureCreated();

                // Seed test data if needed
                SeedTestData(db);
            });
        }

        /// <summary>
        /// Seed initial test data into the database
        /// </summary>
        private void SeedTestData(AppDbContext context)
        {
            // Clear existing data
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            // Additional test seed data can be added here if needed
            // Example: Create 1 admin user, 1 instructor, and 1 student
        }
    }
}
