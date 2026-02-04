using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;

namespace ProductApi.Application.Services;

public class CategoryService
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoryService(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => _categoryRepository.GetAllAsync(cancellationToken);

    public Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _categoryRepository.GetByIdAsync(id, cancellationToken);

    public Task<CategoryDto?> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return Task.FromResult<CategoryDto?>(null);
        return _categoryRepository.CreateAsync(dto.Name.Trim(), cancellationToken);
    }

    public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return null;
        return await _categoryRepository.UpdateAsync(id, dto.Name.Trim(), cancellationToken);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => _categoryRepository.DeleteAsync(id, cancellationToken);
}
