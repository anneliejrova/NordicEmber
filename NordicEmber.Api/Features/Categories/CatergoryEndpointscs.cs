using Microsoft.AspNetCore.Http.HttpResults;
using NordicEmber.Api.Common;

namespace NordicEmber.Api.Features.Categories;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", GetCategories)
            .WithName(nameof(GetCategories))
            .WithSummary("Get categories")
            .WithDescription("Returns all categories with their products, ordered by facet and name. With ?slug= a list with 0 or 1 category is returned.");

        group.MapGet("/{id:int}", GetCategoryById)
            .WithName(nameof(GetCategoryById))
            .WithSummary("Get category by ID");

        group.MapPost("/", CreateCategory)
            .WithName(nameof(CreateCategory))
            .WithSummary("Create category")
            .WithDescription("The URL slug is generated from the name. FacetId is optional but must refer to an existing facet. A name that results in the same slug as an existing category returns 409.")
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapDelete("/{id:int}", DeleteCategory)
            .WithName(nameof(DeleteCategory))
            .WithSummary("Delete category")
            .WithDescription("The products in the category are not deleted, only their link to the category.");

        return app;
    }

    private static async Task<Ok<List<CategoryDto>>> GetCategories(
        [AsParameters] CategoryQuery query,
        CategoryService service,
        CancellationToken cancellationToken)
    {
        var categories = await service.GetCategoriesAsync(query, cancellationToken);

        return TypedResults.Ok(categories);
    }

    private static async Task<Results<Ok<CategoryDto>, NotFound>> GetCategoryById(
        int id,
        CategoryService service,
        CancellationToken cancellationToken)
    {
        var category = await service.GetCategoryByIdAsync(id, cancellationToken);

        return category is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(category);
    }

    private static async Task<Results<CreatedAtRoute<CategoryDto>, ProblemHttpResult>> CreateCategory(
        CreateCategoryDto dto,
        CategoryService service,
        CancellationToken cancellationToken)
    {
        var result = await service.CreateCategoryAsync(dto, cancellationToken);

        if (!result.IsSuccess)
        {
            return result.Error.ToProblem();
        }

        return TypedResults.CreatedAtRoute(result.Value, nameof(GetCategoryById), new { id = result.Value.Id });
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCategory(
        int id,
        CategoryService service,
        CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteCategoryAsync(id, cancellationToken);

        return deleted
            ? TypedResults.NoContent()
            : TypedResults.NotFound();
    }
}
