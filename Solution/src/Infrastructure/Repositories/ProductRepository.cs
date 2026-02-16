using Domain.Entities;
using Domain.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product> GetOrCreateProductAsync(string name, string vendor, string sourceUrl)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Name == name);

        if (product == null)
        {
            product = new Product
            {
                Name = name,
                Vendor = vendor,
                SourceUrl = sourceUrl,
                CreatedAt = DateTime.UtcNow
            };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        return product;
    }

    public async Task<bool> VersionExistsAsync(int productId, string version)
    {
        return await _context.ProductVersions
            .AnyAsync(v => v.ProductId == productId && v.Version == version);
    }

    public async Task AddVersionAsync(int productId, CollectedVersion version)
    {
        var productVersion = new ProductVersion
        {
            ProductId = productId,
            Version = version.Version,
            ReleaseDate = version.ReleaseDate,
            SourceUrl = version.SourceUrl,
            CreatedAt = DateTime.UtcNow
        };

        _context.ProductVersions.Add(productVersion);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}