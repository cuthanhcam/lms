using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleLMS.Domain.Entities;

namespace SimpleLMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Fluent API configuration for the Course entity.
    /// </summary>
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Table name
            builder.ToTable("Courses");

            // Primary key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(2000);

            builder.Property(c => c.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(c => c.InstructorId)
                .IsRequired();

            builder.Property(c => c.IsPublished)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(c => c.InstructorId)
                .HasDatabaseName("IX_Courses_InstructorId");

            builder.HasIndex(c => c.IsPublished)
                .HasDatabaseName("IX_Courses_IsPublished");

            builder.HasIndex(c => new { c.Title, c.InstructorId })
                .HasDatabaseName("IX_Courses_Title_InstructorId");

            // Relationships
            // Course -> Lessons (one-to-many)
            builder.HasMany(c => c.Lessons)
                .WithOne(l => l.Course)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade); // Delete lessons when course is deleted

            // Course -> Enrollments (one-to-many)
            builder.HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Course -> User (many-to-one) - configured in UserConfiguration
        }
    }
}
