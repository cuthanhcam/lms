using LMS.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Primary Key
            builder.HasKey(u => u.Id);

            // Properties
            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Student");

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.HasIndex(u => u.UserName);

            builder.HasIndex(u => u.Role);

            // Constraints
            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint(
                    "CK_Users_Role",
                      "Role IN ('Admin', 'Instructor', 'Student')"
                );
            });
                
            // Relationships are configured in related entities
        }
    }
}
