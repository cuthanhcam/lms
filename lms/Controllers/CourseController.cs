using lms.Interfaces;
using lms.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using lms.Data;

namespace lms.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseRepository _courseRepository;
        private readonly ApplicationDbContext _context;

        public CourseController(ICourseRepository courseRepository, ApplicationDbContext context)
        {
            _courseRepository = courseRepository;
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var courses = await _courseRepository.GetAllAsync();
            return View(courses);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var course = await _courseRepository.GetByIdAsync(id.Value);
            if (course == null) return NotFound();
            return View(course);
        }

        [Authorize(Roles = SD.Role_Teacher + "," + SD.Role_Admin)]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Teacher + "," + SD.Role_Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Price,CategoryId,Status")] Course course, List<IFormFile> images)
        {
            if (ModelState.IsValid)
            {
                course.TeacherId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value);
                course.Images = new List<CourseImage>();

                if (images != null && images.Any())
                {
                    foreach (var image in images)
                    {
                        if (image.Length > 0)
                        {
                            var imagePath = await SaveImage(image);
                            course.Images.Add(new CourseImage { Url = imagePath });
                        }
                    }
                }

                await _courseRepository.AddAsync(course);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        [Authorize(Roles = SD.Role_Teacher + "," + SD.Role_Admin)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var course = await _courseRepository.GetByIdAsync(id.Value);
            if (course == null || (course.TeacherId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value) && !User.IsInRole(SD.Role_Admin)))
                return Forbid();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Teacher + "," + SD.Role_Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Price,CategoryId,Status")] Course course, List<IFormFile> images, List<int> deleteImages)
        {
            if (id != course.Id) return NotFound();
            if (ModelState.IsValid)
            {
                var existingCourse = await _courseRepository.GetByIdAsync(id);
                if (existingCourse == null || (existingCourse.TeacherId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value) && !User.IsInRole(SD.Role_Admin)))
                    return Forbid();

                existingCourse.Title = course.Title;
                existingCourse.Description = course.Description;
                existingCourse.Price = course.Price;
                existingCourse.CategoryId = course.CategoryId;
                existingCourse.Status = course.Status;

                if (deleteImages != null && deleteImages.Any())
                {
                    var imagesToDelete = existingCourse.Images.Where(i => deleteImages.Contains(i.Id)).ToList();
                    foreach (var image in imagesToDelete)
                    {
                        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.Url.TrimStart('/'));
                        if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
                        existingCourse.Images.Remove(image);
                    }
                }

                if (images != null && images.Any())
                {
                    existingCourse.Images ??= new List<CourseImage>();
                    foreach (var image in images)
                    {
                        if (image.Length > 0)
                        {
                            var imagePath = await SaveImage(image);
                            existingCourse.Images.Add(new CourseImage { Url = imagePath });
                        }
                    }
                }

                await _courseRepository.UpdateAsync(existingCourse);
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", course.CategoryId);
            return View(course);
        }

        [Authorize(Roles = SD.Role_Teacher + "," + SD.Role_Admin)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var course = await _courseRepository.GetByIdAsync(id.Value);
            if (course == null || (course.TeacherId != int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value) && !User.IsInRole(SD.Role_Admin)))
                return Forbid();
            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = SD.Role_Teacher + "," + SD.Role_Admin)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _courseRepository.GetByIdAsync(id);
            if (course != null && (course.TeacherId == int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value) || User.IsInRole(SD.Role_Admin)))
            {
                await _courseRepository.DeleteAsync(id);
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<string> SaveImage(IFormFile image)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(image.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
                throw new Exception("Chỉ chấp nhận file .jpg, .jpeg, .png");

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid().ToString() + extension;
            var savePath = Path.Combine(folderPath, fileName);
            using (var fileStream = new FileStream(savePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }
            return "/images/" + fileName;
        }
    }
}