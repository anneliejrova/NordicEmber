namespace NordicEmber.Api.Data.Entities;

public class Product : IHasSlug
{
    public const int NameMaxLength = 100;
    public const int DescriptionMaxLength = 1000;
    public const int ImageMaxLength = 255;
    public const int UrlSlugMaxLength = NameMaxLength;
    public const int MinPrice = 1;        
    public const int MaxPrice = 10000;

    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required decimal Price { get; set; }
    public string? Image { get; set; }
    public required string UrlSlug { get; set; }
    public ICollection<Category> Categories { get; set; } = [];
}
