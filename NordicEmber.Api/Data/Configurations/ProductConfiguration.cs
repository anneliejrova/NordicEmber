using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NordicEmber.Api.Data.Entities;

namespace NordicEmber.Api.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(product => product.Name).HasMaxLength(Product.NameMaxLength);
        builder.Property(product => product.Description).HasMaxLength(Product.DescriptionMaxLength);
        builder.Property(product => product.Image).HasMaxLength(Product.ImageMaxLength);
        builder.Property(product => product.UrlSlug).HasMaxLength(Product.UrlSlugMaxLength);

        builder.Property(product => product.Price).HasPrecision(10, 2);

        builder.HasIndex(product => product.UrlSlug).IsUnique();
    }
}