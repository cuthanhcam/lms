using LMS.Application.DTOs.Lessons;
using LMS.Application.Exceptions;
using LMS.Application.Interfaces;
using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Shared.Constants;

namespace LMS.Application.Services
{
    public class LessonService : ILessonService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LessonService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LessonDto> CreateAsync(Guid courseId, CreateLessonRequest request, Guid userId, string userRole)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);

            if (course == null)
            {
                throw new NotFoundException(nameof(Course), courseId);
            }

            // Check permissions
            if (userRole != Roles.Admin && course.CreatedBy != userId)
            {
                throw new ForbiddenException("You can only add lessons to your own courses");
            }

            // Create lesson using factory method
            var lesson = Lesson.Create(
                title: request.Title,
                content: request.Content,
                order: request.Order
            );
            
            // Add lesson to course using domain method
            course.AddLesson(lesson);

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();

            return MapToLessonDto(lesson);
        }

        public async Task<LessonDto> UpdateAsync(Guid id, UpdateLessonRequest request, Guid userId, string userRole)
        {
            var lesson = await _unitOfWork.Lessons.GetByIdWithCourseAsync(id);

            if (lesson == null)
            {
                throw new NotFoundException(nameof(Lesson), id);
            }

            // Check permissions
            if (userRole != Roles.Admin && lesson.Course?.CreatedBy != userId)
            {
                throw new ForbiddenException("You can only update lessons in your own courses");
            }

            // Use domain method to update lesson
            lesson.UpdateDetails(request.Title, request.Content, request.Order);

            _unitOfWork.Lessons.Update(lesson);
            await _unitOfWork.SaveChangesAsync();

            return MapToLessonDto(lesson);
        }

        public async Task DeleteAsync(Guid id, Guid userId, string userRole)
        {
            var lesson = await _unitOfWork.Lessons.GetByIdWithCourseAsync(id);

            if (lesson == null)
            {
                throw new NotFoundException(nameof(Lesson), id);
            }

            // Check permissions
            if (userRole != Roles.Admin && lesson.Course?.CreatedBy != userId)
            {
                throw new ForbiddenException("You can only delete lessons from your own courses");
            }

            // Soft delete using domain method
            lesson.Delete();
            _unitOfWork.Lessons.Update(lesson);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<LessonDto>> GetByCourseIdAsync(Guid courseId)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(courseId);

            if (course == null)
            {
                throw new NotFoundException(nameof(Course), courseId);
            }

            var lessons = await _unitOfWork.Lessons.GetByCourseIdAsync(courseId);
            return lessons.Select(MapToLessonDto);
        }

        public async Task<LessonDto> GetByIdAsync(Guid id)
        {
            var lesson = await _unitOfWork.Lessons.GetByIdAsync(id);

            if (lesson == null)
            {
                throw new NotFoundException(nameof(Lesson), id);
            }

            return MapToLessonDto(lesson);
        }

        private LessonDto MapToLessonDto(Lesson lesson)
        {
            return new LessonDto
            {
                Id = lesson.Id,
                CourseId = lesson.CourseId,
                Title = lesson.Title,
                Content = lesson.Content,
                Order = lesson.Order
            };
        }
    }
}
