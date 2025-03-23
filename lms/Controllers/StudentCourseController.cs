using lms.Interfaces;
using lms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace lms.Controllers
{
    [Authorize(Roles = SD.Role_Student)]
    public class StudentCourseController : Controller
    {
        private readonly IStudentCourseRepository _studentCourseRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ICourseRepository _courseRepository;

        public StudentCourseController(
            IStudentCourseRepository studentCourseRepository,
            ICartItemRepository cartItemRepository,
            ICourseRepository courseRepository)
        {
            _studentCourseRepository = studentCourseRepository;
            _cartItemRepository = cartItemRepository;
            _courseRepository = courseRepository;
        }

        public async Task<IActionResult> Index()
        {
            int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var enrolledCourses = await _studentCourseRepository.GetEnrolledCoursesByStudentIdAsync(studentId);
            return View(enrolledCourses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null || !course.Status)
                return NotFound();

            if (await _studentCourseRepository.ExistsAsync(studentId, courseId))
            {
                TempData["Error"] = "Bạn đã đăng ký khóa học này.";
                return RedirectToAction("Index", "Course");
            }

            var studentCourse = new StudentCourse
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollDate = DateTime.Now
            };
            await _studentCourseRepository.AddAsync(studentCourse);

            var cartItem = await _cartItemRepository.GetCartItemsByStudentIdAsync(studentId)
                .ContinueWith(t => t.Result.FirstOrDefault(ci => ci.CourseId == courseId));
            if (cartItem != null)
            {
                await _cartItemRepository.DeleteAsync(cartItem.Id);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}