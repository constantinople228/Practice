using Domain.Entities;
using Domain.Models;

namespace Infrastructure.Repositories;

public interface IProductRepository
{
    Task<Product> GetOrCreateProductAsync(string name, string vendor, string sourceUrl);
    Task<bool> VersionExistsAsync(int productId, string version);
    Task AddVersionAsync(int productId, CollectedVersion version);
    Task SaveChangesAsync();
}