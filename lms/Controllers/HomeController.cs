using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using lms.Models;
using lms.Interfaces;
using System.Threading.Tasks;

namespace lms.Controllers;

public class HomeController : Controller
{
    private readonly ICourseRepository _courseRepository;

    public HomeController(ICourseRepository courseRepository)
    {
        _courseRepository = courseRepository;
    }

    public async Task<IActionResult> Index()
    {
        var courses = await _courseRepository.GetAllAsync();
        return View(courses);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
