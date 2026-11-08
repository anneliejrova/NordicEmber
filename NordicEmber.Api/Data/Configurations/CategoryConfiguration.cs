using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NordicEmber.Api.Data.Entities;

namespace NordicEmber.Api.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(category => category.Name).HasMaxLength(Category.NameMaxLength);
        builder.Property(category => category.Description).HasMaxLength(Category.DescriptionMaxLength);
        builder.Property(category => category.Image).HasMaxLength(Category.ImageMaxLength);
        builder.Property(category => category.UrlSlug).HasMaxLength(Category.UrlSlugMaxLength);

        builder.HasIndex(category => category.UrlSlug).IsUnique();

        builder.HasOne(category => category.Facet)
            .WithMany(facet => facet.Categories)
            .HasForeignKey(category => category.FacetId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
