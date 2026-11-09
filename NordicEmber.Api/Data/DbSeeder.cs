using Microsoft.EntityFrameworkCore;
using NordicEmber.Api.Common;
using NordicEmber.Api.Data.Entities;

namespace NordicEmber.Api.Data;

public static class DbSeeder
{
    // Anropas av Update-Database i Package Manager Console
    public static void Seed(DbContext context)
    {
        if (context.Set<Facet>().Any())
        {
            return;
        }

        context.Set<Facet>().AddRange(BuildSeedData());
        context.SaveChanges();
    }

    // Anropas när appen migrerar asynkront
    public static async Task SeedAsync(DbContext context, CancellationToken cancellationToken)
    {
        if (await context.Set<Facet>().AnyAsync(cancellationToken))
        {
            return;
        }

        context.Set<Facet>().AddRange(BuildSeedData());
        await context.SaveChangesAsync(cancellationToken);
    }

    private static List<Facet> BuildSeedData()
    {
        // 1. Produkter först, eftersom kategorierna ska peka på dem
        var yirgacheffe = CreateProduct("Etiopien Yirgacheffe", 149, "Blommig och citrusfrisk med toner av jasmin.");
        var kenya = CreateProduct("Kenya AA", 159, "Livlig syra med smak av svarta vinbär.");
        var huila = CreateProduct("Colombia Huila", 129, "Balanserad med karamell och röda äpplen.");
        var cerrado = CreateProduct("Brasilien Cerrado", 119, "Nötig och chokladig med låg syra.");
        var cajamarca = CreateProduct("Peru Cajamarca", 135, "Mjuk och söt med toner av honung.");
        var sumatra = CreateProduct("Sumatra Mandheling", 139, "Fyllig och jordig med kryddiga toner.");
        var espressoBlend = CreateProduct("Klassisk Espressoblend", 135, "Kraftig crema och mörk choklad.");
        var lungoMild = CreateProduct("Lungo Mild", 125, "Rund och mild för en längre kopp.");
        var decafColombia = CreateProduct("Koffeinfri Colombia", 139, "All smak, inget koffein.");
        var coldBrewCoarse = CreateProduct("Cold Brew Grovmalet", 129, "Grovmalet för kallbryggning.");

        // 2. Facetter med sina kategorier, som pekar på produkterna
        return
        [
            CreateFacet("Rostgrad", 1, allowsMultiple: false,
            [
                CreateCategory("Ljusrost", [yirgacheffe, kenya]),
                CreateCategory("Mellanrost", [huila, cerrado, cajamarca, lungoMild, decafColombia]),
                CreateCategory("Mörkrost", [sumatra, espressoBlend, coldBrewCoarse]),
            ]),
            CreateFacet("Ursprung", 2, allowsMultiple: true,
            [
                CreateCategory("Afrika", [yirgacheffe, kenya]),
                CreateCategory("Sydamerika", [huila, cerrado, cajamarca, decafColombia]),
                CreateCategory("Asien", [sumatra]),
            ]),
            CreateFacet("Bryggmetod", 3, allowsMultiple: true,
            [
                CreateCategory("Espresso", [cerrado, espressoBlend, lungoMild]),
                CreateCategory("Lungo", [espressoBlend, lungoMild]),
                CreateCategory("Bryggkaffe", [yirgacheffe, kenya, huila, decafColombia]),
                CreateCategory("French Press", [huila, cajamarca, sumatra]),
                CreateCategory("Mokabryggare", [cerrado]),
                CreateCategory("Cold Brew", [kenya, coldBrewCoarse]),
            ]),
            CreateFacet("Form", 4, allowsMultiple: false,
            [
                CreateCategory("Hela bönor", [yirgacheffe, kenya, cerrado, cajamarca, sumatra, espressoBlend]),
                CreateCategory("Malet", [huila, lungoMild, decafColombia, coldBrewCoarse]),
            ]),
            CreateFacet("Märkning", 5, allowsMultiple: true,
            [
                CreateCategory("Ekologiskt", [yirgacheffe, cajamarca]),
                CreateCategory("Koffeinfritt", [decafColombia]),
            ]),
        ];
    }

    // Hjälpmetoder: namnet skrivs en gång, sluggen skapas från det
    private static Product CreateProduct(string name, decimal price, string description) => new()
    {
        Name = name,
        Price = price,
        Description = description,
        UrlSlug = SlugGenerator.Generate(name),
    };

    private static Category CreateCategory(string name, List<Product> products) => new()
    {
        Name = name,
        UrlSlug = SlugGenerator.Generate(name),
        Products = products,
    };

    private static Facet CreateFacet(string name, int sortOrder, bool allowsMultiple, List<Category> categories) => new()
    {
        Name = name,
        SortOrder = sortOrder,
        AllowsMultiple = allowsMultiple,
        UrlSlug = SlugGenerator.Generate(name),
        Categories = categories,
    };
}