using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Services;

namespace ProductApi.WebApi.Controllers;

[ApiController]
[Route("product-management/product-categories")]
[ApiExplorerSettings(GroupName = "product-management")]
public class ProductCategoriesController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public ProductCategoriesController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories(CancellationToken cancellationToken = default)
    {
        var list = await _categoryService.GetAllAsync(cancellationToken);
        return Ok(list);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id, CancellationToken cancellationToken = default)
    {
        var category = await _categoryService.GetByIdAsync(id, cancellationToken);
        if (category == null) return NotFound();
        return Ok(category);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProductApi.WebApi.DTOs.ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> CreateCategory(
        [FromBody] CreateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryService.CreateAsync(dto, cancellationToken);
        if (category == null)
            return BadRequest(new ProductApi.WebApi.DTOs.ErrorResponseDto { Message = "Name is required." });
        return StatusCode(StatusCodes.Status201Created, category);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProductApi.WebApi.DTOs.ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(
        int id,
        [FromBody] UpdateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        var category = await _categoryService.UpdateAsync(id, dto, cancellationToken);
        if (category == null)
            return BadRequest(new ProductApi.WebApi.DTOs.ErrorResponseDto { Message = "Category not found or name is required." });
        return Ok(category);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken = default)
    {
        if (!await _categoryService.DeleteAsync(id, cancellationToken))
            return NotFound();
        return NoContent();
    }
}
