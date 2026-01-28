using LMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Data.Configurations
{
    /// <summary>
    /// EF Core configuration for Enrollment entity
    /// Configures properties, relationships, indexes, and enum conversions
    /// </summary>
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // ==================== PROPERTIES ====================

            builder.Property(e => e.EnrollAt)
                .IsRequired();

            // Configure EnrollmentStatus enum (Active, Completed, Cancelled)
            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<int>() // Store enum as int
                .HasDefaultValue(EnrollmentStatus.Active);

            builder.Property(e => e.CompletedAt)
                .IsRequired(false);

            builder.Property(e => e.CancelledAt)
                .IsRequired(false);

            builder.Property(e => e.ProgressPercentage)
                .HasPrecision(5, 2) // Max 100.00
                .HasDefaultValue(0m);

            // ==================== RELATIONSHIPS ====================

            builder.HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // ==================== INDEXES ====================

            // Unique constraint: User can only enroll once per course
            builder.HasIndex(e => new { e.UserId, e.CourseId })
                .IsUnique()
                .HasDatabaseName("IX_Enrollments_UserId_CourseId");

            builder.HasIndex(e => e.UserId)
                .HasDatabaseName("IX_Enrollments_UserId");

            builder.HasIndex(e => e.CourseId)
                .HasDatabaseName("IX_Enrollments_CourseId");

            builder.HasIndex(e => e.EnrollAt)
                .HasDatabaseName("IX_Enrollments_EnrollAt");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Enrollments_Status");

            // ==================== CONSTRAINTS ====================

            builder.ToTable(tb =>
            {
                // Progress percentage must be between 0 and 100
                tb.HasCheckConstraint(
                    "CK_Enrollments_ProgressPercentage",
                    "ProgressPercentage >= 0 AND ProgressPercentage <= 100"
                );

                // Status enum values: 0=Active, 1=Completed, 2=Cancelled
                tb.HasCheckConstraint(
                    "CK_Enrollments_Status",
                    "Status IN (0, 1, 2)"
                );
            });

            // ==================== IGNORE PROPERTIES ====================
            // Domain events are not persisted to database
            builder.Ignore(e => e.DomainEvents);
        }
    }
}
