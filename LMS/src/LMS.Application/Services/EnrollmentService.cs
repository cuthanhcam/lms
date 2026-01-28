using LMS.Application.DTOs.Enrollments;
using LMS.Application.Exceptions;
using LMS.Application.Interfaces;
using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;

namespace LMS.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public EnrollmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<EnrollmentDto> EnrollAsync(Guid courseId, Guid userId)
        {
            var course = await _unitOfWork.Courses.GetByIdWithDetailsAsync(courseId);

            if (course == null)
            {
                throw new NotFoundException(nameof(Course), courseId);
            }

            if (!course.IsPublished)
            {
                throw new BadRequestException("Cannot enroll in unpublished course");
            }

            // Check if already enrolled
            if (await _unitOfWork.Enrollments.IsEnrolledAsync(userId, courseId))
            {
                throw new BadRequestException("You are already enrolled in this course");
            }

            // Create enrollment using factory method
            var enrollment = Enrollment.Create(userId, courseId);

            await _unitOfWork.Enrollments.AddAsync(enrollment);
            await _unitOfWork.SaveChangesAsync();

            return MapToEnrollmentDto(enrollment, course);
        }

        public async Task<IEnumerable<EnrollmentDto>> GetMyCoursesAsync(Guid userId)
        {
            var enrollments = await _unitOfWork.Enrollments.GetByUserIdAsync(userId);
            return enrollments.Select(e => MapToEnrollmentDto(e, e.Course!));
        }

        private EnrollmentDto MapToEnrollmentDto(Enrollment enrollment, Course course)
        {
            return new EnrollmentDto
            {
                Id = enrollment.Id,
                UserId = enrollment.UserId,
                CourseId = enrollment.CourseId,
                CourseTitle = course.Title,
                CourseDescription = course.Description,
                CoursePrice = course.Price.Amount,
                EnrollAt = enrollment.EnrollAt,
                TotalLessons = course.Lessons.Count
            };
        }
    }
}
