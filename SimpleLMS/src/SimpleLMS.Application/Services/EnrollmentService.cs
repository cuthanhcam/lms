using AutoMapper;
using SimpleLMS.Application.Common;
using SimpleLMS.Application.DTOs.Enrollments;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Application.Interfaces.Services;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleLMS.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EnrollmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<EnrollmentDto>> GetByIdAsync(Guid id)
        {
            var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(id);
            if (enrollment == null)
                return Result<EnrollmentDto>.Failure("Enrollment not found");

            var enrollmentDto = _mapper.Map<EnrollmentDto>(enrollment);
            return Result<EnrollmentDto>.Success(enrollmentDto);
        }

        public async Task<Result<IEnumerable<EnrollmentDto>>> GetEnrollmentsByUserAsync(Guid userId)
        {
            var enrollments = await _unitOfWork.Enrollments.GetEnrollmentsByUserAsync(userId);
            var enrollmentDtos = _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
            return Result<IEnumerable<EnrollmentDto>>.Success(enrollmentDtos);
        }

        public async Task<Result<IEnumerable<EnrollmentDto>>> GetEnrollmentsByCourseAsync(Guid courseId)
        {
            var enrollments = await _unitOfWork.Enrollments.GetEnrollmentsByCourseAsync(courseId);
            var enrollmentDtos = _mapper.Map<IEnumerable<EnrollmentDto>>(enrollments);
            return Result<IEnumerable<EnrollmentDto>>.Success(enrollmentDtos);
        }

        public async Task<Result<EnrollmentDto>> EnrollAsync(Guid userId, CreateEnrollmentDto dto)
        {
            try
            {
                // Verify user exists
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                    return Result<EnrollmentDto>.Failure("User not found");

                // Verify course exists and is published
                var course = await _unitOfWork.Courses.GetByIdAsync(dto.CourseId);
                if (course == null)
                    return Result<EnrollmentDto>.Failure("Course not found");

                if (!course.IsPublished)
                    return Result<EnrollmentDto>.Failure("Course is not published");

                var enrollment = new Enrollment(userId, dto.CourseId);

                await _unitOfWork.Enrollments.AddAsync(enrollment);
                await _unitOfWork.SaveChangesAsync();

                var enrollmentDto = _mapper.Map<EnrollmentDto>(enrollment);
                return Result<EnrollmentDto>.Success(enrollmentDto);
            }
            catch (Exception ex)
            {
                return Result<EnrollmentDto>.Failure($"Failed to enroll: {ex.Message}");
            }
        }

        public async Task<Result> UpdateProgressAsync(Guid enrollmentId, Guid userId, UpdateProgressDto dto)
        {
            try
            {
                var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(enrollmentId);
                if (enrollment == null)
                    return Result.Failure("Enrollment not found");

                // Verify ownership
                if (enrollment.UserId != userId)
                    return Result.Failure("You don't have permission to update this enrollment");

                if (enrollment.Status != EnrollmentStatus.Active)
                    return Result.Failure("Cannot update progress for inactive enrollment");

                // Validate progress percentage
                if (dto.ProgressPercentage < 0 || dto.ProgressPercentage > 100)
                    return Result.Failure("Progress percentage must be between 0 and 100");

                enrollment.UpdateProgress(dto.ProgressPercentage);

                // Auto-complete if progress is 100%
                if (dto.ProgressPercentage >= 100)
                {
                    enrollment.Complete();
                }

                await _unitOfWork.Enrollments.UpdateAsync(enrollment);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to update progress: {ex.Message}");
            }
        }

        public async Task<Result> CompleteAsync(Guid enrollmentId, Guid userId)
        {
            try
            {
                var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(enrollmentId);
                if (enrollment == null)
                    return Result.Failure("Enrollment not found");

                // Verify ownership
                if (enrollment.UserId != userId)
                    return Result.Failure("You don't have permission to complete this enrollment");

                enrollment.Complete();

                await _unitOfWork.Enrollments.UpdateAsync(enrollment);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to complete enrollment: {ex.Message}");
            }
        }

        public async Task<Result> CancelAsync(Guid enrollmentId, Guid userId)
        {
            try
            {
                var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(enrollmentId);
                if (enrollment == null)
                    return Result.Failure("Enrollment not found");

                // Verify ownership
                if (enrollment.UserId != userId)
                    return Result.Failure("You don't have permission to cancel this enrollment");

                enrollment.Cancel();

                await _unitOfWork.Enrollments.UpdateAsync(enrollment);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to cancel enrollment: {ex.Message}");
            }
        }
    }
}
