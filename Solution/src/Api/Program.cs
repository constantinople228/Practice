using Infrastructure.Data;
using Infrastructure.Repositories;
using Application.Services;
using Collectors.Collectors;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Api.BackgroundServices;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<VersionSyncService>();

// HTTP Clients for collectors
builder.Services.AddHttpClient<RedisGitHubCollector>();
builder.Services.AddHttpClient<NginxGitHubCollector>();
builder.Services.AddHttpClient<PostgreSqlGitHubCollector>();
builder.Services.AddHttpClient<ElasticsearchGitHubCollector>();
builder.Services.AddHttpClient<GrafanaGitHubCollector>();

// Register collectors
builder.Services.AddSingleton<IVersionCollector, RedisGitHubCollector>();
builder.Services.AddSingleton<IVersionCollector, NginxGitHubCollector>();
builder.Services.AddSingleton<IVersionCollector, PostgreSqlGitHubCollector>();
builder.Services.AddSingleton<IVersionCollector, ElasticsearchGitHubCollector>();
builder.Services.AddSingleton<IVersionCollector, GrafanaGitHubCollector>();

// Background service
builder.Services.AddHostedService<VersionSyncBackgroundService>();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Version Collector API");
    });
}

app.UseHttpsRedirection();
app.MapControllers();

// Apply migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}


app.Run();