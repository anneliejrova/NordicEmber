namespace NordicEmber.Api.Data.Entities;

public interface IHasSlug
{
    int Id { get; }
    string UrlSlug { get; }
}
