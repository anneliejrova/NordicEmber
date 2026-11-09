using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NordicEmber.Api.Common;
using NordicEmber.Api.Data;
using NordicEmber.Api.Data.Entities;

namespace NordicEmber.Api.Features.Products;

public class ProductService(AppDbContext db)
{
    //Get all products or filter by slug if provided
    public async Task<List<ProductDto>> GetProductsAsync(ProductQuery query, CancellationToken cancellationToken)
    {
        var products = db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Slug))
        {
            products = products.Where(product => product.UrlSlug == query.Slug);
        }

        return await products
            .OrderBy(product => product.Name)
            .Select(ProductMappings.Projection)
            .ToListAsync(cancellationToken);
    }

    //Get a single product by its ID
    public async Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await db.Products
            .Where(product => product.Id == id)
            .Select(ProductMappings.Projection)
            .FirstOrDefaultAsync(cancellationToken);
    }

    // Create new product with validation for name and slug uniqueness
    public async Task<Result<ProductDto>> CreateProductAsync(CreateProductDto dto, CancellationToken cancellationToken)
    {
        var slug = SlugGenerator.Generate(dto.Name);

        if (string.IsNullOrEmpty(slug))
        {
            return Result<ProductDto>.Failure(InvalidNameError);
        }

        if (await db.Products.SlugExistsAsync(slug, cancellationToken: cancellationToken))
        {
            return Result<ProductDto>.Failure(DuplicateNameError);
        }

        var product = new Product
        {
            Name = dto.Name.Trim(),
            Description = dto.Description,
            Price = dto.Price,
            Image = dto.Image,
            UrlSlug = slug
        };

        db.Products.Add(product);

        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            return Result<ProductDto>.Failure(DuplicateNameError);
        }

        return Result<ProductDto>.Success(product.ToDto());
    }

    // Delete a product by its ID
    public async Task<bool> DeleteProductAsync(int id, CancellationToken cancellationToken)
    {
        var deletedCount = await db.Products
            .Where(product => product.Id == id)
            .ExecuteDeleteAsync(cancellationToken);

        return deletedCount > 0;
    }

    // Errors returned by CreateProductAsync - Validation of product name and prevention of creating products with invalid names or duplicate names

    private static readonly ResultError InvalidNameError = new(
    ResultErrorType.Validation, nameof(CreateProductDto.Name), "Namnet måste innehålla minst en bokstav eller siffra.");

    private static readonly ResultError DuplicateNameError = new(
        ResultErrorType.Conflict, nameof(CreateProductDto.Name), "Det finns redan en produkt med samma eller liknande namn.");
}
