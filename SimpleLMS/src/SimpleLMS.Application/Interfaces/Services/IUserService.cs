using SimpleLMS.Application.Common;
using SimpleLMS.Application.DTOs.Users;

namespace SimpleLMS.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<Result<UserDto>> GetByIdAsync(Guid id);
        Task<Result<IEnumerable<UserDto>>> GetAllAsync();
        Task<Result<UserDto>> CreateAsync(CreateUserDto dto);
        Task<Result<UserDto>> UpdateAsync(Guid id, UpdateUserDto dto);
        Task<Result> DeleteAsync(Guid id);
        Task<Result<LoginResponseDto>> LoginAsync(LoginDto dto);
    }
}
