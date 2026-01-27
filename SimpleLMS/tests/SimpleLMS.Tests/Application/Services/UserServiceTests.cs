using AutoMapper;
using SimpleLMS.Application.DTOs.Users;
using SimpleLMS.Application.Interfaces.Repositories;
using SimpleLMS.Application.Services;
using SimpleLMS.Domain.Entities;
using SimpleLMS.Domain.Enums;

namespace SimpleLMS.Tests.Application.Services;

/// <summary>
/// Unit tests for the UserService.
/// </summary>
public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _userService = new UserService(_unitOfWorkMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WithExistingUser_ShouldReturnSuccess()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("testuser", "test@test.com", "hash", "Test User", UserRole.Student);
        var userDto = new UserDto { Id = userId, UserName = "testuser" };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.UserName.Should().Be("testuser");
    }

    [Fact]
    public async Task GetByIdAsync_WithNonExistingUser_ShouldReturnFailure()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Be("User not found");
    }

    [Fact]
    public async Task CreateAsync_WithValidData_ShouldCreateUser()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            Username = "newuser",
            Email = "new@test.com",
            Password = "Password123!",
            FullName = "New User",
            Role = UserRole.Student
        };

        _unitOfWorkMock.Setup(u => u.Users.UsernameExistsAsync(createDto.Username))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.Users.EmailExistsAsync(createDto.Email))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.Users.AddAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) => user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns(new UserDto { UserName = createDto.Username });

        // Act
        var result = await _userService.CreateAsync(createDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.UserName.Should().Be(createDto.Username);
        _unitOfWorkMock.Verify(u => u.Users.AddAsync(It.IsAny<User>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WithExistingUsername_ShouldReturnFailure()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            Username = "existing",
            Email = "new@test.com",
            Password = "Password123!",
            FullName = "New User",
            Role = UserRole.Student
        };

        _unitOfWorkMock.Setup(u => u.Users.UsernameExistsAsync(createDto.Username))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.CreateAsync(createDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Username");
    }

    [Fact]
    public async Task CreateAsync_WithExistingEmail_ShouldReturnFailure()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            Username = "newuser",
            Email = "existing@test.com",
            Password = "Password123!",
            FullName = "New User",
            Role = UserRole.Student
        };

        _unitOfWorkMock.Setup(u => u.Users.UsernameExistsAsync(createDto.Username))
            .ReturnsAsync(false);
        _unitOfWorkMock.Setup(u => u.Users.EmailExistsAsync(createDto.Email))
            .ReturnsAsync(true);

        // Act
        var result = await _userService.CreateAsync(createDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Email");
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "testuser",
            Password = "Password123!"
        };

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(loginDto.Password);
        var user = new User(loginDto.Username, "test@test.com", passwordHash, "Test User", UserRole.Student);
        var userDto = new UserDto { UserName = loginDto.Username };

        _unitOfWorkMock.Setup(u => u.Users.GetByUsernameAsync(loginDto.Username))
            .ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _userService.LoginAsync(loginDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task LoginAsync_WithInvalidUsername_ShouldReturnFailure()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "nonexistent",
            Password = "Password123!"
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByUsernameAsync(loginDto.Username))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.LoginAsync(loginDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Invalid");
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnFailure()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Username = "testuser",
            Password = "WrongPassword"
        };

        var passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new User(loginDto.Username, "test@test.com", passwordHash, "Test User", UserRole.Student);

        _unitOfWorkMock.Setup(u => u.Users.GetByUsernameAsync(loginDto.Username))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.LoginAsync(loginDto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Invalid");
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ShouldUpdateUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("testuser", "old@test.com", "hash", "Old Name", UserRole.Student);
        var updateDto = new UpdateUserDto
        {
            FullName = "New Name",
            Email = "new@test.com"
        };

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);
        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns(new UserDto { FullName = updateDto.FullName });

        // Act
        var result = await _userService.UpdateAsync(userId, updateDto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.FullName.Should().Be(updateDto.FullName);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithExistingUser_ShouldDeleteUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("testuser", "test@test.com", "hash", "Test User", UserRole.Student);

        _unitOfWorkMock.Setup(u => u.Users.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _userService.DeleteAsync(userId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
