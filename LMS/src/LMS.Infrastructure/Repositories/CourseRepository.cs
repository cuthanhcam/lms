using LMS.Application.Interfaces.Repositories;
using LMS.Domain.Entities;
using LMS.Infrastructure.Data;
using LMS.Shared.Common;
using Microsoft.EntityFrameworkCore;

namespace LMS.Infrastructure.Repositories
{
    public class CourseRepository : GenericRepository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Course?> GetByIdWithDetailsAsync(Guid id)
        {
            return await _dbSet
                .Include(c => c.CreatedByUser)
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<PagedResult<Course>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            bool? isPublished,
            decimal? minPrice,
            decimal? maxPrice)
        {
            var query = _dbSet
                .Include(c => c.CreatedByUser)
                .Include(c => c.Lessons)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c =>
                    c.Title.Contains(search) ||
                    (c.Description != null && c.Description.Contains(search)));
            }

            if (isPublished.HasValue)
            {
                query = query.Where(c => c.IsPublished == isPublished.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(c => c.Price.Amount >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(c => c.Price.Amount <= maxPrice.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Course>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<IEnumerable<Course>> GetByCreatorAsync(Guid userId)
        {
            return await _dbSet
                .Include(c => c.Lessons)
                .Where(c => c.CreatedBy == userId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> CanPublishAsync(Guid courseId)
        {
            var course = await _dbSet
                .Include(c => c.Lessons)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            return course != null && course.Lessons.Any();
        }
    }
}
