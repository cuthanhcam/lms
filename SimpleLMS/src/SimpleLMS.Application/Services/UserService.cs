using AutoMapper;
using SimpleLMS.Application.Common;
using SimpleLMS.Application.DTOs.Users;
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
    /// User service implementation
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<UserDto>> GetByIdAsync(Guid id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null)
                return Result<UserDto>.Failure("User not found");

            var userDto = _mapper.Map<UserDto>(user);
            return Result<UserDto>.Success(userDto);
        }

        public async Task<Result<IEnumerable<UserDto>>> GetAllAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);
            return Result<IEnumerable<UserDto>>.Success(userDtos);
        }

        public async Task<Result<UserDto>> CreateAsync(CreateUserDto dto)
        {
            try
            {
                // Validate username and email uniqueness
                if (await _unitOfWork.Users.UsernameExistsAsync(dto.Username))
                    return Result<UserDto>.Failure("Username already exists");

                if (await _unitOfWork.Users.EmailExistsAsync(dto.Email))
                    return Result<UserDto>.Failure("Email already exists");

                // Hash password
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

                // Create user
                var user = new User(
                    dto.Username,
                    dto.Email,
                    passwordHash,
                    dto.FullName,
                    dto.Role
                );

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<UserDto>(user);
                return Result<UserDto>.Success(userDto);
            }
            catch (BusinessRuleViolationException ex)
            {
                return Result<UserDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<UserDto>.Failure($"Failed to create user: {ex.Message}");
            }
        }

        public async Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    return Result<UserDto>.Failure("User not found");

                // Check if email is taken by another user
                var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
                if (existingUser != null && existingUser.Id != id)
                    return Result<UserDto>.Failure("Email already exists");

                user.UpdateInfo(dto.Email, dto.FullName);

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                var userDto = _mapper.Map<UserDto>(user);
                return Result<UserDto>.Success(userDto);
            }
            catch (BusinessRuleViolationException ex)
            {
                return Result<UserDto>.Failure(ex.Message);
            }
            catch (Exception ex)
            {
                return Result<UserDto>.Failure($"Failed to update user: {ex.Message}");
            }
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                    return Result.Failure("User not found");

                user.Delete();

                await _unitOfWork.Users.UpdateAsync(user);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Failed to delete user: {ex.Message}");
            }
        }

        public async Task<Result<LoginResponseDto>> LoginAsync(LoginDto dto)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByUsernameAsync(dto.Username);
                if (user == null)
                    return Result<LoginResponseDto>.Failure("Invalid username or password");

                if (!user.IsActive)
                    return Result<LoginResponseDto>.Failure("User account is inactive");

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                    return Result<LoginResponseDto>.Failure("Invalid username or password");

                var userDto = _mapper.Map<UserDto>(user);

                // JWT token will be generated in API Controller layer
                var response = new LoginResponseDto
                {
                    Token = string.Empty, // Will be set by UsersController after successful login
                    User = userDto
                };

                return Result<LoginResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                return Result<LoginResponseDto>.Failure($"Login failed: {ex.Message}");
            }
        }
    }
}
}
