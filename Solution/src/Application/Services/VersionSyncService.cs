using Domain.Interfaces;
using Infrastructure.Repositories;

namespace Application.Services;

public class VersionSyncService
{
    private readonly IEnumerable<IVersionCollector> _collectors;
    private readonly IProductRepository _productRepo;

    public VersionSyncService(
        IEnumerable<IVersionCollector> collectors,
        IProductRepository productRepo)
    {
        _collectors = collectors;
        _productRepo = productRepo;
    }

    public async Task SyncAllAsync()
    {
        foreach (var collector in _collectors)
        {
            try
            {
                var versions = await collector.CollectAsync();

                var product = await _productRepo.GetOrCreateProductAsync(
                    collector.ProductName,
                    "Unknown",
                    "https://github.com");

                foreach (var v in versions)
                {
                    if (!await _productRepo.VersionExistsAsync(product.Id, v.Version))
                    {
                        await _productRepo.AddVersionAsync(product.Id, v);
                    }
                }
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error syncing {collector.ProductName}: {ex.Message}");
            }
        }
    }
}
