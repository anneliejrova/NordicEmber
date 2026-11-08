namespace NordicEmber.Api.Data.Entities;

public class Facet : IHasSlug
{
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 1000;
    public const int ImageMaxLength = 255;
    public const int UrlSlugMaxLength = NameMaxLength;

    public int Id { get; set; }
    public required string Name { get; set; }
    public required int SortOrder { get; set; }
    public string? Description { get; set; }
    public string? Image { get; set; }
    public required string UrlSlug { get; set; }
    public bool AllowsMultiple { get; set; }
    public ICollection<Category> Categories { get; set; } = [];
}
