using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace lms.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên khóa học là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khóa học không được vượt quá 100 ký tự")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Mô tả không được vượt quá 1000 ký tự")]
        public string? Description { get; set; } // Nullable property

        [Range(0, double.MaxValue, ErrorMessage = "Giá khóa học phải lớn hơn hoặc bằng 0")]
        public double Price { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; } // Navigation property

        [ForeignKey("Teacher")]
        public int TeacherId { get; set; }
        public Teacher teacher { get; set; } // Navigation property

        public bool Status { get; set; } = true; // Default value is true (active)
    }
}