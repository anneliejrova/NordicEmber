using System.ComponentModel.DataAnnotations;
using NordicEmber.Api.Data.Entities;
using NordicEmber.Api.Features.Facets;
using NordicEmber.Api.Features.Products;

namespace NordicEmber.Api.Features.Categories;

public record CategoryDto(
    int Id,
    string Name,
    string? Description,
    string? Image,
    string UrlSlug,
    FacetSummaryDto? Facet,
    List<ProductDto> Products
);

public record CreateCategoryDto
{
    [Required(ErrorMessage = "Namn måste anges.")]
    [MaxLength(Category.NameMaxLength,
        ErrorMessage = "Namnet får vara högst {1} tecken.")]
    public string Name { get; init; } = string.Empty;

    [MaxLength(Category.DescriptionMaxLength,
        ErrorMessage = "Beskrivningen får vara högst {1} tecken.")]
    public string? Description { get; init; }

    [MaxLength(Category.ImageMaxLength,
        ErrorMessage = "Bildsökvägen får vara högst {1} tecken.")]
    public string? Image { get; init; }

    public int? FacetId { get; init; }
}

public record CategoryQuery(string? Slug);