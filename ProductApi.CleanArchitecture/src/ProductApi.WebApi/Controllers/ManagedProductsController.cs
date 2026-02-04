using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Services;

namespace ProductApi.WebApi.Controllers;

[ApiController]
[Route("product-management/managed-products")]
[ApiExplorerSettings(GroupName = "product-management")]
public class ManagedProductsController : ControllerBase
{
    private readonly ProductService _productService;

    public ManagedProductsController(ProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResultDto<ProductDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        var result = await _productService.GetPagedAsync(page, pageSize, search, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetProduct(int id, CancellationToken cancellationToken = default)
    {
        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null)
            return NotFound();
        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProductApi.WebApi.DTOs.ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> CreateProduct(
        [FromBody] CreateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var product = await _productService.CreateAsync(dto, cancellationToken);
        if (product == null)
            return BadRequest(new ProductApi.WebApi.DTOs.ErrorResponseDto { Message = "One or more category IDs are invalid." });
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProductApi.WebApi.DTOs.ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> UpdateProduct(
        int id,
        [FromBody] UpdateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var existing = await _productService.GetByIdAsync(id, cancellationToken);
        if (existing == null)
            return NotFound();
        var product = await _productService.UpdateAsync(id, dto, cancellationToken);
        if (product == null)
            return BadRequest(new ProductApi.WebApi.DTOs.ErrorResponseDto { Message = "One or more category IDs are invalid." });
        return Ok(product);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteProduct(int id, CancellationToken cancellationToken = default)
    {
        if (!await _productService.DeleteAsync(id, cancellationToken))
            return NotFound();
        return NoContent();
    }
}
