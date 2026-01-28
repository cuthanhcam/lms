using LMS.Domain.Entities;
using LMS.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LMS.Infrastructure.Data.Configurations
{
    /// <summary>
    /// EF Core configuration for User entity
    /// Configures properties, relationships, indexes, and value object conversions
    /// </summary>
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Primary Key
            builder.HasKey(u => u.Id);

            // ==================== PROPERTIES ====================

            builder.Property(u => u.UserName)
                .IsRequired()
                .HasMaxLength(100);

            // Configure Email Value Object
            // Email is stored as a string but converted to/from Email value object
            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255)
                .HasConversion(
                    // Convert Email to string for database
                    email => email.Value,
                    // Convert string from database to Email
                    value => Email.Create(value)
                );

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            // Configure UserRole enum
            // Store as integer in database (0 = Student, 1 = Instructor, 2 = Admin)
            builder.Property(u => u.Role)
                .IsRequired()
                .HasConversion<int>() // Store enum as int
                .HasDefaultValue(UserRole.Student);

            builder.Property(u => u.IsActive)
                .HasDefaultValue(true);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            // ==================== INDEXES ====================

            builder.HasIndex(u => u.Email)
                .IsUnique()
                .HasDatabaseName("IX_Users_Email");

            builder.HasIndex(u => u.UserName)
                .HasDatabaseName("IX_Users_UserName");

            builder.HasIndex(u => u.Role)
                .HasDatabaseName("IX_Users_Role");

            builder.HasIndex(u => u.IsActive)
                .HasDatabaseName("IX_Users_IsActive");

            // ==================== CONSTRAINTS ====================

            builder.ToTable(tb =>
            {
                // Check constraint for role enum values
                tb.HasCheckConstraint(
                    "CK_Users_Role",
                    "Role IN (0, 1, 2)" // 0=Student, 1=Instructor, 2=Admin
                );
            });

            // ==================== RELATIONSHIPS ====================
            // Relationships are configured in related entities

            // ==================== IGNORE PROPERTIES ====================
            // Domain events are not persisted to database
            builder.Ignore(u => u.DomainEvents);
        }
    }
}
