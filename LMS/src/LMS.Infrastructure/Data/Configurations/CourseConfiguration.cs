using LMS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Data.Configurations
{
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Primary Key
            builder.HasKey(c => c.Id);

            // Properties
            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .HasMaxLength(2000);

            builder.Property(c => c.Price)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(c => c.IsPublished)
                .HasDefaultValue(false);

            builder.Property(c => c.IsDeleted)
                .HasDefaultValue(false);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            // Relationships
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

            // Indexes
            builder.HasIndex(c => c.CreatedBy);

            builder.HasIndex(c => c.IsPublished);

            builder.HasIndex(c => c.IsDeleted);

            builder.HasIndex(c => c.CreatedAt);

            // Constraints
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint(
                    "CK_Courses_Price",
                    "Price >= 0"
                );
            });

            // Query Filters (Soft Delete)
            builder.HasQueryFilter(c => !c.IsDeleted);
        }
    }
}
