using Contracts.Domains.Interfaces;
using Microsoft.EntityFrameworkCore;
using Product.API.Entities;
using ILogger = Serilog.ILogger;
namespace Product.API.Persistence;
public class ProductContextSeed
{
    public static async Task SeedProductAsync ( ProductContext productContext, ILogger logger )
    {
        if (!productContext.Products.Any())
        {
            productContext.AddRange(GetCatalogProducts());
            await productContext.SaveChangesAsync();
            logger.Information("Seeded data for Product DB associated with context {DbContextName}", nameof(ProductContext));

        }
    }
    private static IEnumerable<CatalogProduct> GetCatalogProducts ( )
    {
        return new List<CatalogProduct>
        {
            new CatalogProduct()
            {
                No = "Lotus",
                Name = "Esprit",
                Summary = "Nondisplaced Fracture",
                Description = "Climax NCQ Decription",
                Price = (decimal)2509199.9
            }
        };
    }
}
