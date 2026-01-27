using AutoMapper;
using SimpleLMS.Application.Common;
using SimpleLMS.Application.DTOs.Courses;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Application.Interfaces.Services;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Application.Services
{
    /// <summary>
    /// Course service implementation
    /// </summary>
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CourseService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<CourseDto>> GetByIdAsync(Guid id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
                return Result<CourseDto>.Failure("Course not found");

            var courseDto = _mapper.Map<CourseDto>(course);
            return Result<CourseDto>.Success(courseDto);
        }

        public async Task<Result<CourseDetailDto>> GetDetailByIdAsync(Guid id)
        {
            var course = await _unitOfWork.Courses.GetCourseWithLessonsAsync(id);
            if (course == null)
                return Result<CourseDetailDto>.Failure("Course not found");

            var courseDto = _mapper.Map<CourseDetailDto>(course);
            return Result<CourseDetailDto>.Success(courseDto);
        }

        public async Task<Result<IEnumerable<CourseDto>>> GetAllAsync()
        {
            var courses = await _unitOfWork.Courses.GetAllAsync();
            var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
            return Result<IEnumerable<CourseDto>>.Success(courseDtos);
        }

        public async Task<Result<IEnumerable<CourseDto>>> GetPublishedCoursesAsync()
        {
            var courses = await _unitOfWork.Courses.GetPublishedCoursesAsync();
            var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
            return Result<IEnumerable<CourseDto>>.Success(courseDtos);
        }

        public async Task<Result<IEnumerable<CourseDto>>> GetCoursesByInstructorAsync(Guid instructorId)
        {
            var courses = await _unitOfWork.Courses.GetCoursesByInstructorAsync(instructorId);
            var courseDtos = _mapper.Map<IEnumerable<CourseDto>>(courses);
            return Result<IEnumerable<CourseDto>>.Success(courseDtos);
        }

        public async Task<Result<CourseDto>> CreateAsync(Guid instructorId, CreateCourseDto dto)
        {
            try
            {
                // Verify instructor exists
                var instructor = await _unitOfWork.Users.GetByIdAsync(instructorId);
                if (instructor == null)
                    return Result<CourseDto>.Failure("Instructor not found");

                var course = new Course(
                    dto.Title,
                    dto.Description,
                    dto.Price,
                    instructorId
                );

                await _unitOfWork.Courses.AddAsync(course);
                await _unitOfWork.SaveChangesAsync();

                var courseDto = _mapper.Map<CourseDto>(course);
                return Result<CourseDto>.Success(courseDto);
            }
            catch (BusinessRuleViolationException ex)
            {
                return Result<CourseDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<CourseDto>.Failure($"Failed to create course: {ex.Message}");
            }
        }

        public async Task<Result<CourseDto>> UpdateAsync(Guid id, Guid instructorId, UpdateCourseDto dto)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(id);
                if (course == null)
                    return Result<CourseDto>.Failure("Course not found");

                // Verify ownership
                if (course.InstructorId != instructorId)
                    return Result<CourseDto>.Failure("You don't have permission to update this course");

                course.UpdateInfo(dto.Title, dto.Description, dto.Price);

                await _unitOfWork.Courses.UpdateAsync(course);
                await _unitOfWork.SaveChangesAsync();

                var courseDto = _mapper.Map<CourseDto>(course);
                return Result<CourseDto>.Success(courseDto);
            }
            catch (BusinessRuleViolationException ex)
            {
                return Result<CourseDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<CourseDto>.Failure($"Failed to update course: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(Guid id, Guid instructorId)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(id);
                if (course == null)
                    return Result.Failure("Course not found");

                // Verify ownership
                if (course.InstructorId != instructorId)
                    return Result.Failure("You don't have permission to delete this course");

                course.Delete();

                await _unitOfWork.Courses.UpdateAsync(course);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (BusinessRuleViolationException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete course: {ex.Message}");
            }
        }

        public async Task<Result> PublishAsync(Guid id, Guid instructorId)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetCourseWithLessonsAsync(id);
                if (course == null)
                    return Result.Failure("Course not found");

                // Verify ownership
                if (course.InstructorId != instructorId)
                    return Result.Failure("You don't have permission to publish this course");

                course.Publish();

                await _unitOfWork.Courses.UpdateAsync(course);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (BusinessRuleViolationException ex)
            {
                return Result.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to publish course: {ex.Message}");
            }
        }

        public async Task<Result> UnpublishAsync(Guid id, Guid instructorId)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetByIdAsync(id);
                if (course == null)
                    return Result.Failure("Course not found");

                // Verify ownership
                if (course.InstructorId != instructorId)
                    return Result.Failure("You don't have permission to unpublish this course");

                course.Unpublish();

                await _unitOfWork.Courses.UpdateAsync(course);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to unpublish course: {ex.Message}");
            }
        }
    }
}
