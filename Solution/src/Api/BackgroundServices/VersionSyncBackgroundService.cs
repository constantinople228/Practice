using Application.Services;

namespace Api.BackgroundServices;

public class VersionSyncBackgroundService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<VersionSyncBackgroundService> _logger;

    public VersionSyncBackgroundService(
        IServiceProvider services,
        ILogger<VersionSyncBackgroundService> logger)
    {
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _services.CreateScope();
                var syncService = scope.ServiceProvider.GetRequiredService<VersionSyncService>();

                _logger.LogInformation("Starting version synchronization");
                await syncService.SyncAllAsync();
                _logger.LogInformation("Version synchronization completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during version synchronization");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}