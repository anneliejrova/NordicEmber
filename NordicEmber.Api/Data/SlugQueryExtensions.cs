using Microsoft.EntityFrameworkCore;
using NordicEmber.Api.Data.Entities;

namespace NordicEmber.Api.Data;

public static class SlugQueryExtensions
{
    public static async Task<bool> SlugExistsAsync<TEntity>(
        this IQueryable<TEntity> query,
        string slug,
        int? excludeId = null,
        CancellationToken cancellationToken = default)
        where TEntity : class, IHasSlug
    {
        return await query.AnyAsync(
            entity => entity.UrlSlug == slug && (excludeId == null || entity.Id != excludeId),
            cancellationToken);
    }
}