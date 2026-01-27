using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleLMS.Domain.Entities;

namespace SimpleLMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Fluent API configuration for the Lesson entity.
    /// </summary>
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            // Table name
            builder.ToTable("Lessons");

            // Primary key
            builder.HasKey(l => l.Id);

            // Properties
            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Content)
                .HasMaxLength(5000);

            builder.Property(l => l.VideoUrl)
                .HasMaxLength(500);

            builder.Property(l => l.Order)
                .IsRequired();

            builder.Property(l => l.DurationMinutes)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(l => l.CourseId)
                .IsRequired();

            builder.Property(l => l.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(l => l.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(l => l.UpdatedAt)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(l => l.CourseId)
                .HasDatabaseName("IX_Lessons_CourseId");

            builder.HasIndex(l => new { l.CourseId, l.Order })
                .IsUnique()
                .HasDatabaseName("IX_Lessons_CourseId_Order");

            // Relationships configured in CourseConfiguration
        }
    }
}
