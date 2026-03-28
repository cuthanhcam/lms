using FluentAssertions;
using LMS.Application.DTOs.Courses;
using LMS.Application.Exceptions;
using LMS.Application.Interfaces.Repositories;
using LMS.Application.Services;
using LMS.Application.UnitTests.Builders;
using LMS.Domain.Entities;
using LMS.Shared.Common;
using LMS.Shared.Constants;
using Moq;
using Xunit;

namespace LMS.Application.UnitTests.Services
{
    /// <summary>
    /// Unit tests for CourseService
    /// Test scenarios: CRUD operations, authorization, business rules
    /// </summary>
    public class CourseServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly CourseService _courseService;

        public CourseServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _courseService = new CourseService(_mockUnitOfWork.Object);
        }

        #region CreateAsync Tests

        /// <summary>
        /// Test: Create course with valid data should succeed
        /// </summary>
        [Fact]
        public async Task CreateAsync_WithValidRequest_ShouldReturnCourseDto()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new CreateCourseRequest
            {
                Title = "New Course",
                Description = "Test Description",
                Price = 99.99m
            };

            var createdCourse = new CourseBuilder()
                .WithTitle(request.Title)
                .WithDescription(request.Description)
                .WithPrice(request.Price)
                .WithCreatedBy(userId)
                .WithCreatedByUser(new UserBuilder().WithId(userId).Build())
                .Build();

            _mockUnitOfWork.Setup(x => x.Courses.AddAsync(It.IsAny<Course>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            _mockUnitOfWork.Setup(x => x.Courses.GetByIdWithDetailsAsync(It.IsAny<Guid>()))
                .ReturnsAsync(createdCourse);

            // Act
            var result = await _courseService.CreateAsync(request, userId);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(request.Title);
            result.Description.Should().Be(request.Description);
            result.Price.Should().Be(request.Price);
            result.CreatedBy.Should().Be(userId);
            result.IsPublished.Should().BeFalse(); // Default should be false

            _mockUnitOfWork.Verify(x => x.Courses.AddAsync(It.IsAny<Course>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Test: Create course with negative price should throw BadRequestException
        /// </summary>
        [Fact]
        public async Task CreateAsync_WithNegativePrice_ShouldThrowBadRequestException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new CreateCourseRequest
            {
                Title = "New Course",
                Description = "Test Description",
                Price = -10m // Negative price
            };

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(
                () => _courseService.CreateAsync(request, userId));

            _mockUnitOfWork.Verify(x => x.Courses.AddAsync(It.IsAny<Course>()), Times.Never);
        }

        #endregion

        #region UpdateAsync Tests

        /// <summary>
        /// Test: Instructor updating own course should succeed
        /// </summary>
        [Fact]
        public async Task UpdateAsync_OwnerUpdateOwnCourse_ShouldSucceed()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var request = new UpdateCourseRequest
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Price = 199.99m,
                IsPublished = false
            };

            var existingCourse = new CourseBuilder()
                .WithId(courseId)
                .WithCreatedBy(userId)
                .WithCreatedByUser(new UserBuilder().WithId(userId).Build())
                .Build();

            _mockUnitOfWork.Setup(x => x.Courses.GetByIdWithDetailsAsync(courseId))
                .ReturnsAsync(existingCourse);

            _mockUnitOfWork.Setup(x => x.Courses.Update(It.IsAny<Course>()));

            _mockUnitOfWork.Setup(x => x.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _courseService.UpdateAsync(courseId, request, userId, Roles.Instructor);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(request.Title);
            result.Price.Should().Be(request.Price);

            _mockUnitOfWork.Verify(x => x.Courses.Update(It.IsAny<Course>()), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Test: Instructor updating another user's course should throw ForbiddenException
        /// </summary>
        [Fact]
        public async Task UpdateAsync_InstructorUpdateOtherCourse_ShouldThrowForbiddenException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var otherId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var request = new UpdateCourseRequest
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Price = 199.99m,
                IsPublished = false
            };

            var existingCourse = new CourseBuilder()
                .WithId(courseId)
                .WithCreatedBy(otherId) // Course owned by another user
                .Build();

            _mockUnitOfWork.Setup(x => x.Courses.GetByIdWithDetailsAsync(courseId))
                .ReturnsAsync(existingCourse);

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(
                () => _courseService.UpdateAsync(courseId, request, userId, Roles.Instructor));

            _mockUnitOfWork.Verify(x => x.Courses.Update(It.IsAny<Course>()), Times.Never);
        }

        /// <summary>
        /// Test: Admin can update any course
        /// </summary>
        [Fact]
        public async Task UpdateAsync_AdminUpdateAnyCourse_ShouldSucceed()
        {
            // Arrange
            var adminId = Guid.NewGuid();
            var courseOwnerId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var request = new UpdateCourseRequest
            {
                Title = "Updated by Admin",
                Description = "Updated Description",
                Price = 299.99m,
                IsPublished = false
            };

            var existingCourse = new CourseBuilder()
                .WithId(courseId)
                .WithCreatedBy(courseOwnerId) // Course owned by another user
                .WithCreatedByUser(new UserBuilder().WithId(courseOwnerId).Build())
                .Build();

            _mockUnitOfWork.Setup(x => x.Courses.GetByIdWithDetailsAsync(courseId))
                .ReturnsAsync(existingCourse);

            _mockUnitOfWork.Setup(x => x.Courses.Update(It.IsAny<Course>()));
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _courseService.UpdateAsync(courseId, request, adminId, Roles.Admin);

            // Assert
            result.Should().NotBeNull();
            _mockUnitOfWork.Verify(x => x.Courses.Update(It.IsAny<Course>()), Times.Once);
        }

        /// <summary>
        /// Test: Cannot publish course when it has no lessons
        /// </summary>
        [Fact]
        public async Task UpdateAsync_PublishCourseWithoutLessons_ShouldThrowBadRequestException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var request = new UpdateCourseRequest
            {
                Title = "Course",
                Description = "Description",
                Price = 99.99m,
                IsPublished = true // Attempt to publish
            };

            var existingCourse = new CourseBuilder()
                .WithId(courseId)
                .WithCreatedBy(userId)
                .WithCreatedByUser(new UserBuilder().WithId(userId).Build())
                // No lessons
                .Build();

            _mockUnitOfWork.Setup(x => x.Courses.GetByIdWithDetailsAsync(courseId))
                .ReturnsAsync(existingCourse);

            _mockUnitOfWork.Setup(x => x.Courses.CanPublishAsync(courseId))
                .ReturnsAsync(false); // Cannot publish

            // Act & Assert
            await Assert.ThrowsAsync<BadRequestException>(
                () => _courseService.UpdateAsync(courseId, request, userId, Roles.Instructor));
        }

        /// <summary>
        /// Test: Unpublishing a published course should succeed and should not call CanPublishAsync
        /// </summary>
        [Fact]
        public async Task UpdateAsync_UnpublishPublishedCourse_ShouldSucceedWithoutPublishValidation()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var request = new UpdateCourseRequest
            {
                Title = "Updated Title",
                Description = "Updated Description",
                Price = 150m,
                IsPublished = false
            };

            var existingCourse = new CourseBuilder()
                .WithId(courseId)
                .WithCreatedBy(userId)
                .WithCreatedByUser(new UserBuilder().WithId(userId).Build())
                .AsPublished()
                .Build();

            _mockUnitOfWork.Setup(x => x.Courses.GetByIdWithDetailsAsync(courseId))
                .ReturnsAsync(existingCourse);

            _mockUnitOfWork.Setup(x => x.Courses.Update(It.IsAny<Course>()));
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            var result = await _courseService.UpdateAsync(courseId, request, userId, Roles.Instructor);

            // Assert
            result.Should().NotBeNull();
            result.IsPublished.Should().BeFalse();

            _mockUnitOfWork.Verify(x => x.Courses.CanPublishAsync(It.IsAny<Guid>()), Times.Never);
            _mockUnitOfWork.Verify(x => x.Courses.Update(It.IsAny<Course>()), Times.Once);
        }

        /// <summary>
        /// Test: Update non-existent course should throw NotFoundException
        /// </summary>
        [Fact]
        public async Task UpdateAsync_NonExistentCourse_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var request = new UpdateCourseRequest
            {
                Title = "Updated",
                Price = 99.99m,
                IsPublished = false
            };

            _mockUnitOfWork.Setup(x => x.Courses.GetByIdWithDetailsAsync(courseId))
                .ReturnsAsync((Course?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _courseService.UpdateAsync(courseId, request, userId, Roles.Instructor));
        }

        #endregion

        #region DeleteAsync Tests

        /// <summary>
        /// Test: Admin deleting course should succeed (soft delete)
        /// </summary>
        [Fact]
        public async Task DeleteAsync_AdminDeleteCourse_ShouldSucceed()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseBuilder()
                .WithId(courseId)
                .Build();

            _mockUnitOfWork.Setup(x => x.Courses.GetByIdAsync(courseId))
                .ReturnsAsync(course);

            _mockUnitOfWork.Setup(x => x.Courses.Update(It.IsAny<Course>()));
            _mockUnitOfWork.Setup(x => x.SaveChangesAsync()).ReturnsAsync(1);

            // Act
            await _courseService.DeleteAsync(courseId, Roles.Admin);

            // Assert
            _mockUnitOfWork.Verify(x => x.Courses.Update(It.Is<Course>(c => c.IsDeleted == true)), Times.Once);
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        /// <summary>
        /// Test: Non-admin cannot delete course
        /// </summary>
        [Fact]
        public async Task DeleteAsync_NonAdminDeleteCourse_ShouldThrowForbiddenException()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<ForbiddenException>(
                () => _courseService.DeleteAsync(courseId, Roles.Instructor));

            _mockUnitOfWork.Verify(x => x.Courses.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        /// <summary>
        /// Test: Delete non-existent course should throw NotFoundException
        /// </summary>
        [Fact]
        public async Task DeleteAsync_NonExistentCourse_ShouldThrowNotFoundException()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            _mockUnitOfWork.Setup(x => x.Courses.GetByIdAsync(courseId))
                .ReturnsAsync((Course?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _courseService.DeleteAsync(courseId, Roles.Admin));

            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        #endregion

        #region GetByIdAsync Tests

        /// <summary>
        /// Test: Get course by ID should succeed
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ExistingCourse_ShouldReturnCourseDto()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var course = new CourseBuilder()
                .WithId(courseId)
                .WithTitle("Test Course")
                .WithCreatedByUser(new UserBuilder().Build())
                .Build();

            _mockUnitOfWork.Setup(x => x.Courses.GetByIdWithDetailsAsync(courseId))
                .ReturnsAsync(course);

            // Act
            var result = await _courseService.GetByIdAsync(courseId);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(courseId);
            result.Title.Should().Be("Test Course");
        }

        /// <summary>
        /// Test: Get non-existent course should throw NotFoundException
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_NonExistentCourse_ShouldThrowNotFoundException()
        {
            // Arrange
            var courseId = Guid.NewGuid();

            _mockUnitOfWork.Setup(x => x.Courses.GetByIdWithDetailsAsync(courseId))
                .ReturnsAsync((Course?)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _courseService.GetByIdAsync(courseId));
        }

        /// <summary>
        /// Test: GetById should count only active (non-deleted) lessons
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ShouldCountOnlyNonDeletedLessons()
        {
            // Arrange
            var courseId = Guid.NewGuid();
            var creator = new UserBuilder().WithUserName("instructor").Build();

            var activeLesson = new LessonBuilder().WithCourseId(Guid.Empty).WithOrder(1).Build();
            var deletedLesson = new LessonBuilder().WithCourseId(Guid.Empty).WithOrder(2).AsDeleted().Build();

            var course = new CourseBuilder()
                .WithId(courseId)
                .WithCreatedByUser(creator)
                .WithLesson(activeLesson)
                .WithLesson(deletedLesson)
                .Build();

            _mockUnitOfWork.Setup(x => x.Courses.GetByIdWithDetailsAsync(courseId))
                .ReturnsAsync(course);

            // Act
            var result = await _courseService.GetByIdAsync(courseId);

            // Assert
            result.TotalLessons.Should().Be(1);
        }

        #endregion

        #region Query Tests

        /// <summary>
        /// Test: GetAll should pass query parameters and map paged result correctly
        /// </summary>
        [Fact]
        public async Task GetAllAsync_WithFilters_ShouldReturnMappedPagedResult()
        {
            // Arrange
            var creator = new UserBuilder().WithUserName("creator-user").Build();
            var activeLesson = new LessonBuilder().WithCourseId(Guid.Empty).WithOrder(1).Build();
            var deletedLesson = new LessonBuilder().WithCourseId(Guid.Empty).WithOrder(2).AsDeleted().Build();

            var course = new CourseBuilder()
                .WithCreatedByUser(creator)
                .WithTitle("Advanced C#")
                .WithLesson(activeLesson)
                .WithLesson(deletedLesson)
                .Build();

            var parameters = new CourseQueryParameters
            {
                PageNumber = 2,
                PageSize = 5,
                Search = "c#",
                IsPublished = true,
                MinPrice = 50,
                MaxPrice = 500
            };

            var pagedCourses = new PagedResult<Course>
            {
                Items = new List<Course> { course },
                TotalCount = 11,
                PageNumber = 2,
                PageSize = 5
            };

            _mockUnitOfWork.Setup(x => x.Courses.GetPagedAsync(
                    parameters.PageNumber,
                    parameters.PageSize,
                    parameters.Search,
                    parameters.IsPublished,
                    parameters.MinPrice,
                    parameters.MaxPrice))
                .ReturnsAsync(pagedCourses);

            // Act
            var result = await _courseService.GetAllAsync(parameters);

            // Assert
            result.Should().NotBeNull();
            result.TotalCount.Should().Be(11);
            result.PageNumber.Should().Be(2);
            result.PageSize.Should().Be(5);
            result.Items.Should().HaveCount(1);
            result.Items[0].CreatedByName.Should().Be("creator-user");
            result.Items[0].TotalLessons.Should().Be(1);
        }

        /// <summary>
        /// Test: GetMyCourses should map all courses and use fallback creator name when navigation is null
        /// </summary>
        [Fact]
        public async Task GetMyCourses_ShouldMapCoursesAndUseUnknownFallbackName()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var withCreator = new CourseBuilder()
                .WithCreatedBy(userId)
                .WithCreatedByUser(new UserBuilder().WithId(userId).WithUserName("owner-user").Build())
                .Build();

            var withoutCreator = new CourseBuilder()
                .WithCreatedBy(userId)
                .Build();

            _mockUnitOfWork.Setup(x => x.Courses.GetByCreatorAsync(userId))
                .ReturnsAsync(new List<Course> { withCreator, withoutCreator });

            // Act
            var results = (await _courseService.GetMyCourses(userId)).ToList();

            // Assert
            results.Should().HaveCount(2);
            results.Should().Contain(x => x.CreatedByName == "owner-user");
            results.Should().Contain(x => x.CreatedByName == "Unknown");
        }

        #endregion
    }
}
