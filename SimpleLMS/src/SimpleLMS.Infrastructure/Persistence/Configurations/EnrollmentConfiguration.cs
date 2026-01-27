using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Enums;

namespace SimpleLMS.Infrastructure.Persistence.Configurations
{
    /// <summary>
    /// Fluent API configuration for the Enrollment entity.
    /// </summary>
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            // Table name
            builder.ToTable("Enrollments");

            // Primary key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.UserId)
                .IsRequired();

            builder.Property(e => e.CourseId)
                .IsRequired();

            builder.Property(e => e.EnrolledAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>() // Store enum as string
                .HasDefaultValue(EnrollmentStatus.Active);

            builder.Property(e => e.CompletedAt)
                .IsRequired(false);

            builder.Property(e => e.ProgressPercentage)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.UpdatedAt)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_Enrollments_UserId");

            builder.HasIndex(e => e.CourseId)
                .HasDatabaseName("IX_Enrollments_CourseId");

            builder.HasIndex(e => new { e.UserId, e.CourseId })
                .HasDatabaseName("IX_Enrollments_UserId_CourseId");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Enrollments_Status");

            // Relationships configured in UserConfiguration and CourseConfiguration
        }
    }
}
