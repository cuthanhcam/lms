using LMS.Application.DTOs.Courses;
using LMS.Application.Exceptions;
using LMS.Application.Interfaces;
using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Domain.ValueObjects;
using LMS.Shared.Common;
using LMS.Shared.Constants;

namespace LMS.Application.Services
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CourseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CourseDto> CreateAsync(CreateCourseRequest request, Guid userId)
        {
            if (request.Price < 0)
            {
                throw new BadRequestException("Price must be greater than or equal to 0");
            }

            // Create Money value object with USD currency
            var price = Money.Create(request.Price, "USD");
            
            // Create course using factory method
            var course = Course.Create(
                title: request.Title,
                description: request.Description,
                price: price,
                createdBy: userId
            );

            await _unitOfWork.Courses.AddAsync(course);
            await _unitOfWork.SaveChangesAsync();

            var createdCourse = await _unitOfWork.Courses.GetByIdWithDetailsAsync(course.Id);
            return MapToCourseDto(createdCourse!);
        }

        public async Task<CourseDto> UpdateAsync(Guid id, UpdateCourseRequest request, Guid userId, string userRole)
        {
            var course = await _unitOfWork.Courses.GetByIdWithDetailsAsync(id);

            if (course == null)
            {
                throw new NotFoundException(nameof(Course), id);
            }

            // Check permissions
            if (userRole != Roles.Admin && course.CreatedBy != userId)
            {
                throw new ForbiddenException("You can only update your own courses");
            }

            if (request.Price < 0)
            {
                throw new BadRequestException("Price must be greater than or equal to 0");
            }

            // Validate if trying to publish
            if (request.IsPublished && !course.IsPublished)
            {
                if (!await _unitOfWork.Courses.CanPublishAsync(id))
                {
                    throw new BadRequestException("Cannot publish course without lessons");
                }
            }

            // Use domain methods to update course
            var price = Money.Create(request.Price, "USD");
            course.UpdateDetails(request.Title, request.Description, price);
            
            if (request.IsPublished && !course.IsPublished)
            {
                course.Publish();
            }
            else if (!request.IsPublished && course.IsPublished)
            {
                course.Unpublish();
            }

            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();

            var updatedCourse = await _unitOfWork.Courses.GetByIdWithDetailsAsync(id);
            return MapToCourseDto(updatedCourse!);
        }

        public async Task DeleteAsync(Guid id, string userRole)
        {
            if (userRole != Roles.Admin)
            {
                throw new ForbiddenException("Only admins can delete courses");
            }

            var course = await _unitOfWork.Courses.GetByIdAsync(id);

            if (course == null)
            {
                throw new NotFoundException(nameof(Course), id);
            }

            // Soft delete using domain method
            course.Delete();
            _unitOfWork.Courses.Update(course);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<CourseDto> GetByIdAsync(Guid id)
        {
            var course = await _unitOfWork.Courses.GetByIdWithDetailsAsync(id);

            if (course == null)
            {
                throw new NotFoundException(nameof(Course), id);
            }

            return MapToCourseDto(course);
        }

        public async Task<PagedResult<CourseDto>> GetAllAsync(CourseQueryParameters parameters)
        {
            var pagedResult = await _unitOfWork.Courses.GetPagedAsync(
                parameters.PageNumber,
                parameters.PageSize,
                parameters.Search,
                parameters.IsPublished,
                parameters.MinPrice,
                parameters.MaxPrice);

            return new PagedResult<CourseDto>
            {
                Items = pagedResult.Items.Select(MapToCourseDto).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<IEnumerable<CourseDto>> GetMyCourses(Guid userId)
        {
            var courses = await _unitOfWork.Courses.GetByCreatorAsync(userId);
            return courses.Select(MapToCourseDto);
        }

        private CourseDto MapToCourseDto(Course course)
        {
            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                Price = course.Price.Amount,
                CreatedBy = course.CreatedBy,
                CreatedByName = course.CreatedByUser?.UserName ?? "Unknown",
                IsPublished = course.IsPublished,
                CreatedAt = course.CreatedAt,
                TotalLessons = course.Lessons.Count
            };
        }
    }
}
