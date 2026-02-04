using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain;

namespace ProductApi.Application.Services;

public class ProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;

    public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
    }

    public Task<PagedResultDto<ProductDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
        => _productRepository.GetPagedAsync(page, pageSize, search, cancellationToken);

    public Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _productRepository.GetByIdAsync(id, cancellationToken);

    public async Task<ProductDto?> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var categoryIds = dto.CategoryIds?.Distinct().ToList() ?? new List<int>();
        if (categoryIds.Count > 0 && !await _categoryRepository.ExistsAsync(categoryIds, cancellationToken))
            return null;

        var product = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var created = await _productRepository.AddAsync(product, categoryIds, cancellationToken);
        return await _productRepository.GetByIdAsync(created.Id, cancellationToken);
    }

    public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        var categoryIds = dto.CategoryIds?.Distinct().ToList() ?? new List<int>();
        if (categoryIds.Count > 0 && !await _categoryRepository.ExistsAsync(categoryIds, cancellationToken))
            return null;

        var product = new Product
        {
            Id = id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            UpdatedAt = DateTime.UtcNow
        };
        var updated = await _productRepository.UpdateAsync(id, product, categoryIds, cancellationToken);
        return updated != null ? await _productRepository.GetByIdAsync(id, cancellationToken) : null;
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => _productRepository.DeleteAsync(id, cancellationToken);
}
