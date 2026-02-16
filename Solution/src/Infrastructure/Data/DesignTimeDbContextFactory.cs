using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Infrastructure.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        
        var basePath = FindApiPath();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "ConnectionString 'DefaultConnection' не найден в appsettings.json");
        }

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new AppDbContext(optionsBuilder.Options);
    }

    private string FindApiPath()
    {
        var currentDir = Directory.GetCurrentDirectory();
        var apiPath = Path.Combine(currentDir, "../Api");

        if (Directory.Exists(apiPath) && File.Exists(Path.Combine(apiPath, "appsettings.json")))
        {
            return apiPath;
        }

        var solutionRoot = Path.Combine(currentDir, "..", "..");
        var apiInSrc = Path.Combine(solutionRoot, "src", "Api");

        if (File.Exists(Path.Combine(apiInSrc, "appsettings.json")))
        {
            return apiInSrc;
        }

        throw new FileNotFoundException(
            "Не удалось найти appsettings.json. " +
            $"Искали в: {apiPath} и {apiInSrc}");
    }
}