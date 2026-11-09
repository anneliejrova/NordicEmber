using NordicEmber.Api.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace NordicEmber.Api.Features.Products;

public record ProductDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    string? Image,
    string UrlSlug
 );

public record CreateProductDto
{
    [Required(ErrorMessage = "Namn måste anges.")]
    [MaxLength(Product.NameMaxLength, 
        ErrorMessage = "Namnet får vara högst {1} tecken.")]
    public string Name { get; init; } = string.Empty;

    [MaxLength(Product.DescriptionMaxLength, 
        ErrorMessage = "Beskrivningen får vara högst {1} tecken.")]
    public string? Description { get; init; }

    [Range(Product.MinPrice, Product.MaxPrice, 
        ErrorMessage = "Priset måste vara mellan {1} och {2} kr.")]
    public decimal Price { get; init; }

    [MaxLength(Product.ImageMaxLength, 
        ErrorMessage = "Bildsökvägen får vara högst {1} tecken.")]
    public string? Image { get; init; }
}

public record ProductQuery(string? Slug);