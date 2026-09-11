using FlashCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.Infrastructure.Data;

public class FlashCartDbContext : DbContext
{
    public FlashCartDbContext(
        DbContextOptions<FlashCartDbContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Cart> Carts => Set<Cart>();

    public DbSet<CartItem> CartItems => Set<CartItem>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();


protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

  modelBuilder.Entity<Product>()
        .Property(p => p.Price)
        .HasPrecision(10, 2);
        
    modelBuilder.Entity<Product>()
        .HasOne(p => p.Category)
        .WithMany(c => c.Products)
        .HasForeignKey(p => p.CategoryId);

    modelBuilder.Entity<Cart>()
        .HasOne(c => c.User)
        .WithMany(u => u.Cart)
        .HasForeignKey(c => c.UserId);

    modelBuilder.Entity<CartItem>()
        .HasOne(ci => ci.Cart)
        .WithMany(c => c.CartItems)
        .HasForeignKey(ci => ci.CartId);

    modelBuilder.Entity<CartItem>()
        .HasOne(ci => ci.Product)
        .WithMany()
        .HasForeignKey(ci => ci.ProductId);

    modelBuilder.Entity<Order>()
        .HasOne(o => o.User)
        .WithMany(u => u.Orders)
        .HasForeignKey(o => o.UserId);

    modelBuilder.Entity<OrderItem>()
        .HasOne(oi => oi.Order)
        .WithMany(o => o.OrderItems)
        .HasForeignKey(oi => oi.OrderId);

    modelBuilder.Entity<OrderItem>()
        .HasOne(oi => oi.Product)
        .WithMany()
        .HasForeignKey(oi => oi.ProductId);
}
}