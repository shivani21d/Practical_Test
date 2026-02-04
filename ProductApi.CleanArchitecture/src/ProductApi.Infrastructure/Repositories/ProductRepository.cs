using Microsoft.EntityFrameworkCore;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _db;

    public ProductRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResultDto<ProductDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _db.Products
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(p => p.Name.Contains(search));

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(p => p.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                StockQuantity = p.StockQuantity,
                Categories = p.ProductCategories.Select(pc => new CategoryDto { Id = pc.Category.Id, Name = pc.Category.Name }).ToList()
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ProductDto>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _db.Products
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        return product == null ? null : MapToDto(product);
    }

    public async Task<Product> AddAsync(Product product, IReadOnlyList<int> categoryIds, CancellationToken cancellationToken = default)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync(cancellationToken);
        foreach (var categoryId in categoryIds)
            _db.ProductCategories.Add(new ProductCategory { ProductId = product.Id, CategoryId = categoryId });
        await _db.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product product, IReadOnlyList<int> categoryIds, CancellationToken cancellationToken = default)
    {
        var existing = await _db.Products.Include(p => p.ProductCategories).FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (existing == null)
            return null;

        existing.Name = product.Name;
        existing.Description = product.Description;
        existing.Price = product.Price;
        existing.StockQuantity = product.StockQuantity;
        existing.UpdatedAt = product.UpdatedAt;

        _db.ProductCategories.RemoveRange(existing.ProductCategories);
        foreach (var categoryId in categoryIds)
            _db.ProductCategories.Add(new ProductCategory { ProductId = id, CategoryId = categoryId });

        await _db.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var product = await _db.Products.FindAsync(new object[] { id }, cancellationToken);
        if (product == null)
            return false;
        _db.Products.Remove(product);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    private static ProductDto MapToDto(Product p)
    {
        return new ProductDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            StockQuantity = p.StockQuantity,
            Categories = p.ProductCategories?
                .Select(pc => new CategoryDto { Id = pc.Category.Id, Name = pc.Category.Name })
                .ToList() ?? new List<CategoryDto>()
        };
    }
}
