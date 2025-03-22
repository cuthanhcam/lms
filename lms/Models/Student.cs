using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lms.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Tên sinh viên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(256)]
        public string PasswordHasd { get; set; }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public List<StudentCourse> EnrolledCourses { get; set; } = new List<StudentCourse>();
    }
}