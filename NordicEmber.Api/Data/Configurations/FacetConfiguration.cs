using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NordicEmber.Api.Data.Entities;

namespace NordicEmber.Api.Data.Configurations;

public class FacetConfiguration : IEntityTypeConfiguration<Facet>
{
    public void Configure(EntityTypeBuilder<Facet> builder)
    {
        builder.Property(group => group.Name).HasMaxLength(Facet.NameMaxLength);
        builder.Property(group => group.Description).HasMaxLength(Facet.DescriptionMaxLength);
        builder.Property(group => group.Image).HasMaxLength(Facet.ImageMaxLength);
        builder.Property(group => group.UrlSlug).HasMaxLength(Facet.UrlSlugMaxLength);

        builder.HasIndex(facet => facet.UrlSlug).IsUnique();
    }
}