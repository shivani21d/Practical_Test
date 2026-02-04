using Microsoft.EntityFrameworkCore;
using ProductApi.Domain;

namespace ProductApi.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken cancellationToken = default)
    {
        await db.ProductCategories.ExecuteDeleteAsync(cancellationToken);
        await db.Products.ExecuteDeleteAsync(cancellationToken);
        await db.Categories.ExecuteDeleteAsync(cancellationToken);
        await db.SaveChangesAsync(cancellationToken);

        var categories = new[]
        {
            new Category { Name = "Electronics" },
            new Category { Name = "Clothing" },
            new Category { Name = "Home & Garden" },
            new Category { Name = "Sports" },
            new Category { Name = "Books" },
        };
        db.Categories.AddRange(categories);
        await db.SaveChangesAsync(cancellationToken);

        var now = DateTime.UtcNow;
        var products = new List<Product>
        {
            new Product { Name = "Wireless Headphones", Description = "Noise-cancelling over-ear headphones with 30hr battery.", Price = 129.99m, StockQuantity = 50, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Smart Watch", Description = "Fitness tracking, heart rate, GPS. Water resistant.", Price = 249.99m, StockQuantity = 30, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "USB-C Hub", Description = "7-in-1 adapter with HDMI, USB 3.0, SD card reader.", Price = 45.00m, StockQuantity = 100, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Cotton T-Shirt", Description = "Organic cotton, unisex, multiple colors.", Price = 24.99m, StockQuantity = 200, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Running Shorts", Description = "Lightweight, moisture-wicking, reflective trim.", Price = 34.99m, StockQuantity = 80, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Winter Jacket", Description = "Insulated, water-resistant, hooded.", Price = 159.99m, StockQuantity = 40, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Desk Lamp", Description = "LED, adjustable brightness, USB port.", Price = 39.99m, StockQuantity = 60, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Plant Pot Set", Description = "Ceramic, 3 sizes, with drainage holes.", Price = 29.99m, StockQuantity = 75, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Yoga Mat", Description = "Non-slip, 6mm thick, eco-friendly material.", Price = 32.00m, StockQuantity = 90, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Dumbbells 5kg Pair", Description = "Rubber coated, hex shape, no roll.", Price = 44.99m, StockQuantity = 45, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Programming Guide", Description = "Learn C# and .NET from scratch.", Price = 49.99m, StockQuantity = 120, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Cookbook", Description = "100 quick weekday recipes.", Price = 19.99m, StockQuantity = 85, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Bluetooth Speaker", Description = "Portable, 12hr battery, waterproof.", Price = 59.99m, StockQuantity = 55, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Keyboard Mechanical", Description = "RGB, Cherry MX switches, wired.", Price = 89.99m, StockQuantity = 35, CreatedAt = now, UpdatedAt = now },
            new Product { Name = "Garden Hose", Description = "25m expandable, kink-free, with nozzle.", Price = 36.99m, StockQuantity = 40, CreatedAt = now, UpdatedAt = now },
        };
        db.Products.AddRange(products);
        await db.SaveChangesAsync(cancellationToken);

        var categoryList = await db.Categories.OrderBy(c => c.Id).ToListAsync(cancellationToken);
        var productList = await db.Products.OrderBy(p => p.Id).ToListAsync(cancellationToken);
        int CatId(string name) => categoryList.First(c => c.Name == name).Id;
        int ProdId(string name) => productList.First(p => p.Name == name).Id;

        var productCategories = new List<ProductCategory>
        {
            new ProductCategory { ProductId = ProdId("Wireless Headphones"), CategoryId = CatId("Electronics") },
            new ProductCategory { ProductId = ProdId("Smart Watch"), CategoryId = CatId("Electronics") },
            new ProductCategory { ProductId = ProdId("USB-C Hub"), CategoryId = CatId("Electronics") },
            new ProductCategory { ProductId = ProdId("Cotton T-Shirt"), CategoryId = CatId("Clothing") },
            new ProductCategory { ProductId = ProdId("Running Shorts"), CategoryId = CatId("Clothing") },
            new ProductCategory { ProductId = ProdId("Running Shorts"), CategoryId = CatId("Sports") },
            new ProductCategory { ProductId = ProdId("Winter Jacket"), CategoryId = CatId("Clothing") },
            new ProductCategory { ProductId = ProdId("Desk Lamp"), CategoryId = CatId("Home & Garden") },
            new ProductCategory { ProductId = ProdId("Plant Pot Set"), CategoryId = CatId("Home & Garden") },
            new ProductCategory { ProductId = ProdId("Yoga Mat"), CategoryId = CatId("Sports") },
            new ProductCategory { ProductId = ProdId("Dumbbells 5kg Pair"), CategoryId = CatId("Sports") },
            new ProductCategory { ProductId = ProdId("Programming Guide"), CategoryId = CatId("Books") },
            new ProductCategory { ProductId = ProdId("Cookbook"), CategoryId = CatId("Books") },
            new ProductCategory { ProductId = ProdId("Bluetooth Speaker"), CategoryId = CatId("Electronics") },
            new ProductCategory { ProductId = ProdId("Keyboard Mechanical"), CategoryId = CatId("Electronics") },
            new ProductCategory { ProductId = ProdId("Garden Hose"), CategoryId = CatId("Home & Garden") },
        };
        db.ProductCategories.AddRange(productCategories);
        await db.SaveChangesAsync(cancellationToken);
    }
}
