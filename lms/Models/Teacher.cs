using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace lms.Models
{
    public class Teacher
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên giảng viên là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên giảng viên không được vượt quá 100 ký tự")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(256)]
        public string PasswordHash { get; set; }

        public List<Course> Courses { get; set; } = new List<Course>();
    }
}