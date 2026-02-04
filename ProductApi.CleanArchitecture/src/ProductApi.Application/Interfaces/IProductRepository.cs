using ProductApi.Application.DTOs;
using ProductApi.Domain;

namespace ProductApi.Application.Interfaces;

public interface IProductRepository
{
    Task<PagedResultDto<ProductDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default);
    Task<ProductDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Product> AddAsync(Product product, IReadOnlyList<int> categoryIds, CancellationToken cancellationToken = default);
    Task<Product?> UpdateAsync(int id, Product product, IReadOnlyList<int> categoryIds, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
