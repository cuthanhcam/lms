using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace lms.Models
{
    [Index(nameof(StudentId), nameof(CourseId), IsUnique = true)]
    public class StudentCourse
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public ApplicationUser Student { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Required]
        public DateTime EnrollDate { get; set; } = DateTime.Now;
    }
}