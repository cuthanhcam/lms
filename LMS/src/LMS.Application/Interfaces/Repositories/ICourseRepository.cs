using LMS.Domain.Entities;
using LMS.Shared.Common;

namespace LMS.Application.Interfaces.Repositories
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<Course?> GetByIdWithDetailsAsync(Guid id);
        Task<PagedResult<Course>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            bool? isPublished,
            decimal? minPrice,
            decimal? maxPrice);
        Task<IEnumerable<Course>> GetByCreatorAsync(Guid userId);
        Task<bool> CanPublishAsync(Guid courseId);
    }
}
