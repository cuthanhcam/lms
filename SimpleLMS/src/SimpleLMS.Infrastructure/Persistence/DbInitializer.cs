using Microsoft.EntityFrameworkCore;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedDataAsync(AppDbContext context)
        {
            // Check if data already exists
            if (await context.Users.AnyAsync())
            {
                return; // Database has been seeded
            }

            // Create Admin User
            var admin = new User(
                "admin",
                "admin@gmail.com",
                BCrypt.Net.BCrypt.HashPassword("Password@123"),
                "System Administrator",
                UserRole.Admin
            );

            // Create Instructor Users
            var instructor1 = new User(
                "instructor1",
                "instructor1@gmail.com",
                BCrypt.Net.BCrypt.HashPassword("Password@123"),
                "John Doe",
                UserRole.Instructor
            );

            var instructor2 = new User(
                "instructor2",
                "instructor2@gmail.com",
                BCrypt.Net.BCrypt.HashPassword("Password@123"),
                "Jane Smith",
                UserRole.Instructor
            );

            // Create Student Users
            var student1 = new User(
                "student1",
                "student1@gmail.com",
                BCrypt.Net.BCrypt.HashPassword("Password@123"),
                "Alice Nguyen",
                UserRole.Student
            );

            var student2 = new User(
                "student2",
                "student2@gmail.com",
                BCrypt.Net.BCrypt.HashPassword("Password@123"),
                "Bob Tran",
                UserRole.Student
            );

            // Add users to context
            await context.Users.AddRangeAsync(admin, instructor1, instructor2, student1, student2);
            await context.SaveChangesAsync();

            // Create Courses
            var course1 = new Course(
                "ASP.NET Core Fundamentals",
                "Learn the basics of building web applications with ASP.NET Core. This course covers MVC, Web API, Entity Framework Core, and more.",
                499000m,
                instructor1.Id
            );

            var course2 = new Course(
                "Advanced C# Programming",
                "Master advanced C# concepts including LINQ, async/await, delegates, events, and design patterns.",
                699000m,
                instructor1.Id
            );

            var course3 = new Course(
                "Clean Architecture & DDD",
                "Learn how to build maintainable applications using Clean Architecture, Domain-Driven Design, and SOLID principles.",
                899000m,
                instructor2.Id
            );

            var course4 = new Course(
                "React for Beginners",
                "Start your journey with React. Learn components, hooks, state management, and build real-world applications.",
                599000m,
                instructor2.Id
            );

            // Add courses to context
            await context.Courses.AddRangeAsync(course1, course2, course3, course4);
            await context.SaveChangesAsync();

            // Create Lessons for Course 1 (ASP.NET Core Fundamentals)
            var course1Lessons = new[]
            {
            new Lesson("Introduction to ASP.NET Core", "Overview of ASP.NET Core framework and its features", "https://example.com/video1", 1, 15, course1.Id),
            new Lesson("MVC Pattern", "Understanding Model-View-Controller architecture", "https://example.com/video2", 2, 30, course1.Id),
            new Lesson("Dependency Injection", "Learn about DI container and service lifetime", "https://example.com/video3", 3, 25, course1.Id),
            new Lesson("Entity Framework Core Basics", "Introduction to EF Core and Code First approach", "https://example.com/video4", 4, 40, course1.Id),
            new Lesson("Building REST APIs", "Create RESTful web services with Web API", "https://example.com/video5", 5, 35, course1.Id)
        };

            // Create Lessons for Course 2 (Advanced C#)
            var course2Lessons = new[]
            {
            new Lesson("LINQ Fundamentals", "Query data using Language Integrated Query", "https://example.com/video6", 1, 30, course2.Id),
            new Lesson("Async/Await Pattern", "Asynchronous programming in C#", "https://example.com/video7", 2, 35, course2.Id),
            new Lesson("Delegates and Events", "Understanding delegates, events, and event handlers", "https://example.com/video8", 3, 30, course2.Id),
            new Lesson("Design Patterns", "Common design patterns in C#", "https://example.com/video9", 4, 45, course2.Id)
        };

            // Create Lessons for Course 3 (Clean Architecture)
            var course3Lessons = new[]
            {
            new Lesson("Clean Architecture Overview", "Introduction to Clean Architecture principles", "https://example.com/video10", 1, 25, course3.Id),
            new Lesson("Domain Layer", "Building the core domain with entities and business rules", "https://example.com/video11", 2, 40, course3.Id),
            new Lesson("Application Layer", "Use cases and application services", "https://example.com/video12", 3, 35, course3.Id),
            new Lesson("Infrastructure Layer", "Implementing repositories and external services", "https://example.com/video13", 4, 40, course3.Id),
            new Lesson("DDD Patterns", "Aggregates, Value Objects, and Domain Events", "https://example.com/video14", 5, 50, course3.Id)
        };

            // Add lessons to context
            await context.Lessons.AddRangeAsync(course1Lessons);
            await context.Lessons.AddRangeAsync(course2Lessons);
            await context.Lessons.AddRangeAsync(course3Lessons);
            await context.SaveChangesAsync();

            // Publish courses (Course 1, 2, 3 have lessons, so they can be published)
            course1.Publish();
            course2.Publish();
            course3.Publish();

            // Course 4 is not published (no lessons yet)

            await context.SaveChangesAsync();

            // Create Enrollments
            var enrollment1 = new Enrollment(student1.Id, course1.Id);
            enrollment1.UpdateProgress(60);

            var enrollment2 = new Enrollment(student1.Id, course2.Id);
            enrollment2.UpdateProgress(95); // Almost complete

            var enrollment3 = new Enrollment(student2.Id, course1.Id);
            enrollment3.UpdateProgress(30);

            var enrollment4 = new Enrollment(student2.Id, course3.Id);
            enrollment4.UpdateProgress(15);

            await context.Enrollments.AddRangeAsync(enrollment1, enrollment2, enrollment3, enrollment4);
            await context.SaveChangesAsync();

            Console.WriteLine("Database seeded successfully!");
        }
    }
}
