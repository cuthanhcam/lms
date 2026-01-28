using LMS.Domain.Entities;

namespace LMS.Application.Interfaces.Identity
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
