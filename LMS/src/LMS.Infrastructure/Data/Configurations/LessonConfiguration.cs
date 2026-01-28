using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Data.Configurations
{
    /// <summary>
    /// EF Core configuration for Lesson entity
    /// Configures properties, relationships, indexes, and query filters
    /// </summary>
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            // Primary Key
            builder.HasKey(l => l.Id);

            // ==================== PROPERTIES ====================

            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Content)
                .IsRequired()
                .HasMaxLength(10000); // Limit content size

            builder.Property(l => l.Order)
                .IsRequired();

            builder.Property(l => l.IsDeleted)
                .HasDefaultValue(false);

            // ==================== RELATIONSHIPS ====================

            builder.HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== INDEXES ====================

            // Unique constraint: Order must be unique within a course
            builder.HasIndex(l => new { l.CourseId, l.Order })
                .IsUnique()
                .HasDatabaseName("IX_Lessons_CourseId_Order");

            builder.HasIndex(l => l.CourseId)
                .HasDatabaseName("IX_Lessons_CourseId");

            builder.HasIndex(l => l.IsDeleted)
                .HasDatabaseName("IX_Lessons_IsDeleted");

            // ==================== CONSTRAINTS ====================

            builder.ToTable(tb =>
            {
                // Order must be positive
                tb.HasCheckConstraint(
                    "CK_Lessons_Order",
                    "[Order] > 0"
                );
            });

            // ==================== QUERY FILTERS ====================
            // Global query filter for soft delete
            builder.HasQueryFilter(l => !l.IsDeleted);

            // ==================== IGNORE PROPERTIES ====================
            // Domain events are not persisted to database
            builder.Ignore(l => l.DomainEvents);
        }
    }
}
