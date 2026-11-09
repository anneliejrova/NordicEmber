using System.Linq.Expressions;
using NordicEmber.Api.Data.Entities;

namespace NordicEmber.Api.Features.Products;

public static class ProductMappings
{
    public static readonly Expression<Func<Product, ProductDto>> Projection =
        product => new ProductDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.Image,
            product.UrlSlug
        );

    private static readonly Func<Product, ProductDto> CompiledProjection = Projection.Compile();

    public static ProductDto ToDto(this Product product) => CompiledProjection(product);
}
