using ProductApi.Application.DTOs;

namespace ProductApi.Application.Interfaces;

public interface ICategoryRepository
{
    Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<CategoryDto?> CreateAsync(string name, CancellationToken cancellationToken = default);
    Task<CategoryDto?> UpdateAsync(int id, string name, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
