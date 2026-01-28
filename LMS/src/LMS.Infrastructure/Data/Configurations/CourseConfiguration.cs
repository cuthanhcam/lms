using LMS.Domain.Entities;
using LMS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Data.Configurations
{
    /// <summary>
    /// EF Core configuration for Course entity
    /// Configures properties, relationships, indexes, and value object conversions
    /// </summary>
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Primary Key
            builder.HasKey(c => c.Id);

            // ==================== PROPERTIES ====================

            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(2000);

            // Configure Money Value Object
            // Money is stored as two columns: Price_Amount and Price_Currency
            builder.OwnsOne(c => c.Price, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("Price")
                    .HasPrecision(18, 2)
                    .IsRequired();

                price.Property(p => p.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3) // ISO 4217 currency codes are 3 characters
                    .IsRequired()
                    .HasDefaultValue("USD");
            });

            builder.Property(c => c.IsPublished)
                .HasDefaultValue(false);

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            // ==================== RELATIONSHIPS ====================

            builder.HasOne(c => c.CreatedByUser)
                .WithMany(u => u.CreatedCourses)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Lessons)
                .WithOne(l => l.Course)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== INDEXES ====================
            // Indexes improve query performance for common queries

            builder.HasIndex(c => c.CreatedBy)
                .HasDatabaseName("IX_Courses_CreatedBy");

            builder.HasIndex(c => c.IsPublished)
                .HasDatabaseName("IX_Courses_IsPublished");

            builder.HasIndex(c => c.IsDeleted)
                .HasDatabaseName("IX_Courses_IsDeleted");

            builder.HasIndex(c => c.CreatedAt)
                .HasDatabaseName("IX_Courses_CreatedAt");

            // Composite index for common query: published and not deleted
            builder.HasIndex(c => new { c.IsPublished, c.IsDeleted })
                .HasDatabaseName("IX_Courses_IsPublished_IsDeleted");

            // ==================== CONSTRAINTS ====================

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint(
                    "CK_Courses_Price",
                    "Price >= 0"
                );
            });

            // ==================== QUERY FILTERS ====================
            // Global query filter for soft delete
            builder.HasQueryFilter(c => !c.IsDeleted);

            // ==================== IGNORE PROPERTIES ====================
            // Domain events are not persisted to database
            builder.Ignore(c => c.DomainEvents);
        }
    }
}
