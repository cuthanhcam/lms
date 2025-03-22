using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using lms.Models;
using Microsoft.EntityFrameworkCore;

namespace lms.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CartItem>()
                .HasIndex(c => new { c.StudentId, c.CourseId })
                .IsUnique();     

            modelBuilder.Entity<StudentCourse>()
                .HasIndex(sc => new { sc.StudentId, sc.CourseId })
                .IsUnique();
        }
    }
}