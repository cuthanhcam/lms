using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace lms.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Required(ErrorMessage = "Tên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        public List<Course> Courses { get; set; } = new List<Course>();
        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<StudentCourse> EnrolledCourses { get; set; } = new List<StudentCourse>();
    }
}