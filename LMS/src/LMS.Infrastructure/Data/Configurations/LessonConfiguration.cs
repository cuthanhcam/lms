using LMS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Data.Configurations
{
    public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
    {
        public void Configure(EntityTypeBuilder<Lesson> builder)
        {
            // Primary Key
            builder.HasKey(l => l.Id);

            // Properties
            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Content)
                .IsRequired();

            builder.Property(l => l.Order)
                .IsRequired();

            builder.Property(l => l.IsDeleted)
                .HasDefaultValue(false);

            // Relationships
            builder.HasOne(l => l.Course)
                .WithMany(c => c.Lessons)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(l => new { l.CourseId, l.Order })
                .IsUnique();

            builder.HasIndex(l => l.CourseId);

            // Query Filters (Soft Delete)
            builder.HasQueryFilter(l => !l.IsDeleted);
        }
    }
}
