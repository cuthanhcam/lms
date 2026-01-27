using AutoMapper;
using SimpleLMS.Application.Common;
using SimpleLMS.Application.DTOs.Lessons;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Application.Interfaces.Services;
using SimpleLMS.Domain.Common;
using SimpleLMS.Domain.Entities;

namespace SimpleLMS.Application.Services
{
    /// <summary>
    /// Lesson service implementation
    /// </summary>
    public class LessonService : ILessonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LessonService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<LessonDto>> GetByIdAsync(Guid id)
        {
            var lesson = await _unitOfWork.Lessons.GetByIdAsync(id);
            if (lesson == null)
                return Result<LessonDto>.Failure("Lesson not found");

            var lessonDto = _mapper.Map<LessonDto>(lesson);
            return Result<LessonDto>.Success(lessonDto);
        }

        public async Task<Result<IEnumerable<LessonDto>>> GetLessonsByCourseAsync(Guid courseId)
        {
            var lessons = await _unitOfWork.Lessons.GetLessonsByCourseAsync(courseId);
            var lessonDtos = _mapper.Map<IEnumerable<LessonDto>>(lessons);
            return Result<IEnumerable<LessonDto>>.Success(lessonDtos);
        }

        public async Task<Result<LessonDto>> CreateAsync(Guid courseId, Guid instructorId, CreateLessonDto dto)
        {
            try
            {
                // Verify course exists and belongs to instructor
                var course = await _unitOfWork.Courses.GetByIdAsync(courseId);
                if (course == null)
                    return Result<LessonDto>.Failure("Course not found");

                if (course.InstructorId != instructorId)
                    return Result<LessonDto>.Failure("You don't have permission to add lessons to this course");

                var lesson = new Lesson(
                    dto.Title,
                    dto.Content,
                    dto.VideoUrl,
                    dto.Order,
                    dto.DurationMinutes,
                    courseId
                );

                await _unitOfWork.Lessons.AddAsync(lesson);
                await _unitOfWork.SaveChangesAsync();

                var lessonDto = _mapper.Map<LessonDto>(lesson);
                return Result<LessonDto>.Success(lessonDto);
            }
            catch (Exception ex)
            {
                return Result<LessonDto>.Failure($"Failed to create lesson: {ex.Message}");
            }
        }

        public async Task<Result<LessonDto>> UpdateAsync(Guid id, Guid instructorId, UpdateLessonDto dto)
        {
            try
            {
                var lesson = await _unitOfWork.Lessons.GetByIdAsync(id);
                if (lesson == null)
                    return Result<LessonDto>.Failure("Lesson not found");

                // Verify course ownership
                var course = await _unitOfWork.Courses.GetByIdAsync(lesson.CourseId);
                if (course == null)
                    return Result<LessonDto>.Failure("Course not found");

                if (course.InstructorId != instructorId)
                    return Result<LessonDto>.Failure("You don't have permission to update this lesson");

                lesson.UpdateInfo(dto.Title, dto.Content, dto.VideoUrl, dto.DurationMinutes);

                await _unitOfWork.Lessons.UpdateAsync(lesson);
                await _unitOfWork.SaveChangesAsync();

                var lessonDto = _mapper.Map<LessonDto>(lesson);
                return Result<LessonDto>.Success(lessonDto);
            }
            catch (Exception ex)
            {
                return Result<LessonDto>.Failure($"Failed to update lesson: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(Guid id, Guid instructorId)
        {
            try
            {
                var lesson = await _unitOfWork.Lessons.GetByIdAsync(id);
                if (lesson == null)
                    return Result.Failure("Lesson not found");

                // Verify course ownership
                var course = await _unitOfWork.Courses.GetByIdAsync(lesson.CourseId);
                if (course == null)
                    return Result.Failure("Course not found");

                if (course.InstructorId != instructorId)
                    return Result.Failure("You don't have permission to delete this lesson");

                lesson.Delete();

                await _unitOfWork.Lessons.UpdateAsync(lesson);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete lesson: {ex.Message}");
            }
        }
    }
}
