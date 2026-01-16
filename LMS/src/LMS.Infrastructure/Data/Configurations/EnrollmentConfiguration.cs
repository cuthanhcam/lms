using LMS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Data.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties
            builder.Property(e => e.EnrollAt)
                .IsRequired();

            // Relationships
            builder.HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => new { e.UserId, e.CourseId })
                .IsUnique();

            builder.HasIndex(e => e.UserId);

            builder.HasIndex(e => e.CourseId);

            builder.HasIndex(e => e.EnrollAt);
        }
    }
}
