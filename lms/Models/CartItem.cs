using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lms.Models
{
    [Index(nameof(StudentId), nameof(CourseId), IsUnique = true)]
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public ApplicationUser Student { get; set; } // Thay bằng ApplicationUser

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}