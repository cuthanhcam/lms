using AutoMapper;
using SimpleLMS.Application.DTOs.Courses;
using SimpleLMS.Application.DTOs.Enrollments;
using SimpleLMS.Application.DTOs.Lessons;
using SimpleLMS.Application.DTOs.Users;
using SimpleLMS.Domain.Entities;

namespace SimpleLMS.Application.Mappings
{
    /// <summary>
    /// Defines a mapping profile for configuring object-object mappings using AutoMapper.
    /// </summary>
    /// <remarks>Inherit from this class to specify mapping configurations between source and destination
    /// types. Mapping profiles help organize and group related mapping rules for maintainability and reuse.</remarks>
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.ToString()));

            CreateMap<CreateUserDto, User>();

            // Course mappings
            CreateMap<Course, CourseDto>()
                .ForMember(dest => dest.InstructorName,
                    opt => opt.MapFrom(src =>
                        src.Instructor != null ? src.Instructor.FullName : string.Empty))
                .ForMember(dest => dest.TotalLessons,
                    opt => opt.MapFrom(src => src.Lessons.Count))
                .ForMember(dest => dest.TotalDurationMinutes,
                    opt => opt.MapFrom(src =>
                        src.Lessons.Sum(l => l.DurationMinutes)));

            CreateMap<Course, CourseDetailDto>()
                .ForMember(dest => dest.InstructorName,
                    opt => opt.MapFrom(src =>
                        src.Instructor != null ? src.Instructor.FullName : string.Empty))
                .ForMember(dest => dest.TotalLessons,
                    opt => opt.MapFrom(src =>
                        src.Lessons.Count))
                .ForMember(dest => dest.TotalDurationMinutes,
                    opt => opt.MapFrom(src =>
                        src.Lessons.Sum(l => l.DurationMinutes)))
                .ForMember(dest => dest.Lessons,
                    opt => opt.MapFrom(src => src.Lessons));

            CreateMap<CreateCourseDto, Course>();

            // Lesson mappings
            CreateMap<Lesson, LessonDto>();
            CreateMap<CreateLessonDto, Lesson>();

            // Enrollment mappings
            CreateMap<Enrollment, EnrollmentDto>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src =>
                        src.User != null ? src.User.FullName : string.Empty))
                .ForMember(dest => dest.CourseTitle,
                    opt => opt.MapFrom(src =>
                        src.Course != null ? src.Course.Title : string.Empty))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => src.Status.ToString()));

            CreateMap<CreateEnrollmentDto, Enrollment>();
        }
    }
}
