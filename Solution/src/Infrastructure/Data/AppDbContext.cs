using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductVersion> ProductVersions { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductVersion>()
            .HasIndex(pv => new { pv.ProductId, pv.Version })
            .IsUnique();

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired();
            entity.Property(p => p.CreatedAt).HasDefaultValueSql("NOW()");
        });

        modelBuilder.Entity<ProductVersion>(entity =>
        {
            entity.HasKey(pv => pv.Id);
            entity.Property(pv => pv.Version).IsRequired();
            entity.Property(pv => pv.CreatedAt).HasDefaultValueSql("NOW()");

            entity.HasOne(pv => pv.Product)
                .WithMany(p => p.Versions)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}