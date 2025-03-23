using lms.Interfaces;
using lms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace lms.Controllers
{
    [Authorize(Roles = SD.Role_Student)]
    public class CartController : Controller
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly ICourseRepository _courseRepository;

        public CartController(ICartItemRepository cartItemRepository, ICourseRepository courseRepository)
        {
            _cartItemRepository = cartItemRepository;
            _courseRepository = courseRepository;
        }

        public async Task<IActionResult> Index()
        {
            int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var cartItems = await _cartItemRepository.GetCartItemsByStudentIdAsync(studentId);
            return View(cartItems);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int courseId)
        {
            int studentId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var course = await _courseRepository.GetByIdAsync(courseId);
            if (course == null || !course.Status)
                return NotFound();

            if (await _cartItemRepository.ExistsAsync(studentId, courseId))
            {
                TempData["Error"] = "Khóa học đã có trong giỏ hàng.";
                return RedirectToAction("Index", "Course");
            }

            var cartItem = new CartItem
            {
                StudentId = studentId,
                CourseId = courseId
            };
            await _cartItemRepository.AddAsync(cartItem);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int id)
        {
            var cartItem = await _cartItemRepository.GetByIdAsync(id);
            if (cartItem == null || cartItem.StudentId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Forbid();

            await _cartItemRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}