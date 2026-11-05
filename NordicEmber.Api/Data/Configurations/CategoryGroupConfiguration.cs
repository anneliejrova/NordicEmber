using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NordicEmber.Api.Data.Entities;

namespace NordicEmber.Api.Data.Configurations;

public class CategoryGroupConfiguration : IEntityTypeConfiguration<CategoryGroup>
{
    public void Configure(EntityTypeBuilder<CategoryGroup> builder)
    {
        builder.Property(group => group.Name).HasMaxLength(CategoryGroup.NameMaxLength);
        builder.Property(group => group.Description).HasMaxLength(CategoryGroup.DescriptionMaxLength);
        builder.Property(group => group.Image).HasMaxLength(CategoryGroup.ImageMaxLength);
        builder.Property(group => group.UrlSlug).HasMaxLength(CategoryGroup.UrlSlugMaxLength);

        builder.HasIndex(group => group.UrlSlug).IsUnique();
    }
}