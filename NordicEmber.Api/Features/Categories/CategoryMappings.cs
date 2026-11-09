using System.Linq.Expressions;
using NordicEmber.Api.Data.Entities;
using NordicEmber.Api.Features.Facets;
using NordicEmber.Api.Features.Products;

namespace NordicEmber.Api.Features.Categories;

public static class CategoryMappings
{
    public static readonly Expression<Func<Category, CategoryDto>> Projection =
        category => new CategoryDto(
            category.Id,
            category.Name,
            category.Description,
            category.Image,
            category.UrlSlug,
            category.Facet == null
                ? null
                : new FacetSummaryDto(category.Facet.Id, category.Facet.Name, category.Facet.UrlSlug),
            category.Products
                .AsQueryable()
                .OrderBy(product => product.Name)
                .Select(ProductMappings.Projection)
                .ToList()
        );

}