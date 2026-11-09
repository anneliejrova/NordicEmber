using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NordicEmber.Api.Common;
using NordicEmber.Api.Data;
using NordicEmber.Api.Data.Entities;

namespace NordicEmber.Api.Features.Categories;

public class CategoryService(AppDbContext db)
{
    // Get all categories or filter by slug if provided
    public async Task<List<CategoryDto>> GetCategoriesAsync(CategoryQuery query, CancellationToken cancellationToken)
    {
        var categories = db.Categories.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Slug))
        {
            categories = categories.Where(category => category.UrlSlug == query.Slug);
        }

        return await categories
            .OrderBy(category => category.Facet == null)         // Categories without facet go last
            .ThenBy(category => category.Facet!.SortOrder)       // Then facet order
            .ThenBy(category => category.Name)                   // Then by name
            .Select(CategoryMappings.Projection)
            .ToListAsync(cancellationToken);
    }

    // Get a category by ID
    public async Task<CategoryDto?> GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await db.Categories
            .Where(category => category.Id == id)
            .Select(CategoryMappings.Projection)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // Create new category with validation for name, slug uniqueness and facet
    public async Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        var slug = SlugGenerator.Generate(dto.Name);

        if (string.IsNullOrEmpty(slug))
        {
            return Result<CategoryDto>.Failure(InvalidNameError);
        }

        // The facet must exist if one is given
        if (dto.FacetId is not null
            && !await db.Facets.AnyAsync(facet => facet.Id == dto.FacetId, cancellationToken))
        {
            return Result<CategoryDto>.Failure(FacetNotFoundError(dto.FacetId.Value));   
        }

        if (await db.Categories.SlugExistsAsync(slug, cancellationToken: cancellationToken))
        {
            return Result<CategoryDto>.Failure(DuplicateNameError);
        }

        var category = new Category
        {
            Name = dto.Name.Trim(),
            Description = dto.Description,
            Image = dto.Image,
            UrlSlug = slug,
            FacetId = dto.FacetId
        };

        db.Categories.Add(category);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            return Result<CategoryDto>.Failure(DuplicateNameError);
        }

        // Read back from the database so the facet is included
        var created = await GetCategoryByIdAsync(category.Id, cancellationToken);

        return Result<CategoryDto>.Success(created!);
    }

    // Delete a category by ID. Products remain, only links are removed.
    public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var deletedCount = await db.Categories
            .Where(category => category.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deletedCount > 0;
    }

    // Errors returned by CreateCategoryAsync - Validation of category name and facet, and prevention of creating categories with invalid names, duplicate names or a missing facet

    private static readonly ResultError InvalidNameError = new(
        ResultErrorType.Validation, nameof(CreateCategoryDto.Name), "Namnet måste innehålla minst en bokstav eller siffra.");

       private static ResultError FacetNotFoundError(int facetId) => new(
        ResultErrorType.Validation,
        nameof(CreateCategoryDto.FacetId),
        $"Det finns ingen facett med ID {facetId}. Ange ett befintligt facett-ID, eller utelämna fältet.");

    private static readonly ResultError DuplicateNameError = new(
        ResultErrorType.Conflict, nameof(CreateCategoryDto.Name), "Det finns redan en kategori med samma eller liknande namn.");
}