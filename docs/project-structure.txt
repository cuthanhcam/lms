# Learning Management System (LMS) - Project Structure

## Cấu trúc thư mục

```plaintext
LMS/
│-- Controllers/
│   │-- HomeController.cs
│   │-- AccountController.cs
│   │-- CourseController.cs
│   │-- CategoryController.cs
│   │-- TeacherController.cs
│   │-- StudentController.cs
│   │-- CartController.cs
│
│-- Views/
│   │-- Shared/ (Layout, Partial Views)
│   │-- Home/ (Index, About)
│   │-- Account/ (Login, Register, Profile)
│   │-- Course/ (Index, Details, Create, Edit, Delete)
│   │-- Category/ (Index, Create, Edit, Delete)
│   │-- Teacher/ (Dashboard, ManageCourses)
│   │-- Student/ (Dashboard, MyCourses)
│   │-- Cart/ (Index, Checkout)
│
│-- Models/
│   │-- Category.cs
│   │-- Course.cs
│   │-- Teacher.cs
│   │-- Student.cs
│   │-- CartItem.cs
│   │-- StudentCourse.cs
│   │-- ApplicationUser.cs
│
│-- Interfaces/
│   │-- ICategoryRepository.cs
│   │-- ICourseRepository.cs
│   │-- ITeacherRepository.cs
│   │-- IStudentRepository.cs
│   │-- ICartRepository.cs
│
│-- Repositories/
│   │-- CategoryRepository.cs
│   │-- CourseRepository.cs
│   │-- TeacherRepository.cs
│   │-- StudentRepository.cs
│   │-- CartRepository.cs
│
│-- Data/
│   │-- ApplicationDbContext.cs
│   │-- SeedData.cs
│
│-- wwwroot/ (CSS, JS, Images)
│-- appsettings.json
│-- Program.cs
│-- Startup.cs
```
