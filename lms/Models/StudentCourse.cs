using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace lms.Models
{
    [Index(nameof(StudentId), nameof(CourseId), IsUnique = true)] // Unique index on StudentId and CourseId
    public class StudentCourse
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student Student { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; }

        [Required]
        public DateTime EnrollDate { get; set; } = DateTime.Now; // Default to current date
    }
}