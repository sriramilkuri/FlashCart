using FlashCart.ProductService.Application.Common.Interfaces;
using FlashCart.ProductService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.ProductService.Infrastructure.Data;

public class ProductDbContext :
    DbContext,
    IProductDbContext
{
    public ProductDbContext(
        DbContextOptions<ProductDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products
        => Set<Product>();

    public DbSet<Category> Categories
        => Set<Category>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>()
            .HasOne<Category>()
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.CategoryId);

        modelBuilder.Entity<Product>()
            .Property(x => x.Price)
            .HasPrecision(18, 2);
    }
}