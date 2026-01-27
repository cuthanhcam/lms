using SimpleLMS.Domain.Entities;

namespace SimpleLMS.API.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
