using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace lms.Models
{
    public class CourseImage
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Đường dẫn hình ảnh là bắt buộc")]
        [StringLength(500, ErrorMessage = "Đường dẫn hình ảnh không được vượt quá 500 ký tự")]
        public string Url { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; } // Navigation property
    }
}