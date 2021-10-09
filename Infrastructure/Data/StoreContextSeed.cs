using System.Text.Json;
using Core.Entities;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAsync(StoreContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                 if(!context.ProductBrands.Any())
                 {
                     var brandsData = File.ReadAllText("../Infrastructure/Data/SeedData/brands.json");
                     var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);

                     foreach (var item in brands)
                     {
                         await context.ProductBrands.AddAsync(item);
                     }
                     await context.SaveChangesAsync();
                 }

                 if(!context.ProductType.Any())
                 {
                     var typesData = File.ReadAllText("../Infrastructure/Data/SeedData/types.json");
                     var types = JsonSerializer.Deserialize<List<ProductType>>(typesData);

                     foreach (var item in types)
                     {
                         await context.ProductType.AddAsync(item);
                     }
                     await context.SaveChangesAsync();
                 }

                 if(!context.Products.Any())
                 {
                     var productsData = File.ReadAllText("../Infrastructure/Data/SeedData/products.json");
                     var products = JsonSerializer.Deserialize<List<Product>>(productsData);

                     foreach (var item in products)
                     {
                         await context.Products.AddAsync(item);
                     }
                     await context.SaveChangesAsync();
                 }

            }
            catch (System.Exception ex)
            {
                
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(ex.Message);
            }
        }
    }
}