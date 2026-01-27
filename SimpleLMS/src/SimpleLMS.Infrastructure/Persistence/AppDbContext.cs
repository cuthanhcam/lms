using Microsoft.EntityFrameworkCore;
using SimpleLMS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users => Set<User>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Lesson> Lessons => Set<Lesson>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply configurations
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            // Soft delete filter
            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<Course>().HasQueryFilter(c => !c.IsDeleted);
            modelBuilder.Entity<Lesson>().HasQueryFilter(l => !l.IsDeleted);
            modelBuilder.Entity<Enrollment>().HasQueryFilter(e => !e.IsDeleted);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Automatically update UpdatedAt timestamp
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.Entity is Domain.Common.BaseEntity entity)
                {
                    entity.GetType().GetProperty("UpdatedAt")?
                        .SetValue(entity, DateTime.UtcNow);
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
