using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;   

namespace NordicEmber.Api.Common;

public static partial class SlugGenerator
{
    public static string Generate(string text)
    {
        var slug = text.ToLowerInvariant();

        slug = slug.Normalize(NormalizationForm.FormD);     //lägger specialtecken ovanför bokstav bredvid

        var builder = new StringBuilder();

        //ta bort tecken bredvid som tillhört bokstav
        foreach (var character in slug)                                   
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)   
            {
                builder.Append(character);
            }
        }

        slug = builder.ToString();

        slug = NonAlphanumericRegex().Replace(slug, "-");   // udda tecken blir bindestreck

        slug = slug.Trim('-');                              // bort med bindestreck i kanterna

        return slug;
    }

    [GeneratedRegex("[^a-z0-9]+")]                          
    private static partial Regex NonAlphanumericRegex();    
}