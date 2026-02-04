using Microsoft.EntityFrameworkCore;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain;
using ProductApi.Infrastructure.Data;

namespace ProductApi.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
            .ToListAsync(cancellationToken);
    }

    public async Task<CategoryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        return category == null ? null : new CategoryDto { Id = category.Id, Name = category.Name };
    }

    public async Task<CategoryDto?> CreateAsync(string name, CancellationToken cancellationToken = default)
    {
        var category = new Category { Name = name };
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(cancellationToken);
        return new CategoryDto { Id = category.Id, Name = category.Name };
    }

    public async Task<CategoryDto?> UpdateAsync(int id, string name, CancellationToken cancellationToken = default)
    {
        var category = await _db.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (category == null) return null;
        category.Name = name;
        await _db.SaveChangesAsync(cancellationToken);
        return new CategoryDto { Id = category.Id, Name = category.Name };
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var category = await _db.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (category == null) return false;
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<bool> ExistsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
            return true;
        var count = await _db.Categories.Where(c => idList.Contains(c.Id)).CountAsync(cancellationToken);
        return count == idList.Count;
    }
}
