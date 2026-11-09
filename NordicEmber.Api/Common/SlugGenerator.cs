using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace NordicEmber.Api.Common;

public static partial class SlugGenerator
{
    /// <summary>
    /// Creates slug from text. The slug is never longer than the text,
    /// which UrlSlugMaxLength = NameMaxLength relies on.
    /// </summary>
    public static string Generate(string text)
    {
        var slug = text.ToLowerInvariant();                 

        slug = slug.Normalize(NormalizationForm.FormD);     // Moves accents from above the letter to beside it (ö → o¨)

        var builder = new StringBuilder();

        // Removes accents that belonged to letters, keeping everything else
        foreach (var character in slug)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(character);
            }
        }

        slug = builder.ToString();

        slug = NonAlphanumericRegex().Replace(slug, "-");   // One or more odd characters in a row become one hyphen

        slug = slug.Trim('-');                              // Remove hyphens at the start and end

        return slug;
    }

    [GeneratedRegex("[^a-z0-9]+")]
    private static partial Regex NonAlphanumericRegex();
}