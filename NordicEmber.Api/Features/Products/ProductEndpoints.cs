using Microsoft.AspNetCore.Http.HttpResults;
using NordicEmber.Api.Common;

namespace NordicEmber.Api.Features.Products;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", GetProducts)
    .WithName(nameof(GetProducts))
    .WithSummary("Get products")
    .WithDescription("Returns all products sorted by name. With ?slug= a list with 0 or 1 product is returned.");

        group.MapGet("/{id:int}", GetProductById)
            .WithName(nameof(GetProductById))
            .WithSummary("Get product by ID");

        group.MapPost("/", CreateProduct)
            .WithName(nameof(CreateProduct))
            .WithSummary("Create product")
            .WithDescription("The URL slug is generated from the name. A name that results in the same slug as an existing product returns 409.")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:int}", DeleteProduct)
            .WithName(nameof(DeleteProduct))
            .WithSummary("Delete product");

        return app;
    }

    private static async Task<Ok<List<ProductDto>>> GetProducts(
        [AsParameters] ProductQuery query,
        ProductService service,
        CancellationToken cancellationToken)
    {
        var products = await service.GetProductsAsync(query, cancellationToken);

        return TypedResults.Ok(products);
    }

    private static async Task<Results<Ok<ProductDto>, NotFound>> GetProductById(
        int id,
        ProductService service,
        CancellationToken cancellationToken)
    {
        var product = await service.GetProductByIdAsync(id, cancellationToken);

        return product is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(product);
    }

    private static async Task<Results<CreatedAtRoute<ProductDto>, ProblemHttpResult>> CreateProduct(
        CreateProductDto dto,
        ProductService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateProductAsync(dto, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error.ToProblem();
        }

        return TypedResults.CreatedAtRoute(result.Value, nameof(GetProductById), new { id = result.Value.Id });
    }

    private static async Task<Results<NoContent, NotFound>> DeleteProduct(
        int id,
        ProductService service,
        CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteProductAsync(id, cancellationToken);

        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}
