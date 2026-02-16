using Application.DTOs;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProductsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts()
    {
        return await _context.Products
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Vendor = p.Vendor
            })
            .ToListAsync();
    }

    [HttpGet("{productId}/versions")]
    public async Task<ActionResult<PagedResult<ProductVersionDto>>> GetVersions(
        int productId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 20;
        if (pageSize > 100) pageSize = 100;

        var product = await _context.Products.FindAsync(productId);
        if (product == null)
        {
            return NotFound($"Product with ID {productId} not found");
        }

        var query = _context.ProductVersions
            .Where(v => v.ProductId == productId)
            .OrderByDescending(v => v.ReleaseDate);

        var total = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(v => new ProductVersionDto
            {
                Version = v.Version,
                ReleaseDate = v.ReleaseDate,
                SourceUrl = v.SourceUrl
            })
            .ToListAsync();

        var result = new PagedResult<ProductVersionDto>
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Items = items
        };

        return Ok(result);
    }
}