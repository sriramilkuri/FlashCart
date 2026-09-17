using FlashCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using FlashCart.Application.Common.Interfaces;

namespace FlashCart.Infrastructure.Data;

public class FlashCartDbContext :
    DbContext,
    IApplicationDbContext
{
    public FlashCartDbContext(
        DbContextOptions<FlashCartDbContext> options)
        : base(options)
    {
    }

public DbSet<ProcessedMessage> ProcessedMessages
    => Set<ProcessedMessage>();
    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<Category> Categories => Set<Category>();

    public DbSet<Product> Products => Set<Product>();

    public DbSet<User> Users => Set<User>();

    public DbSet<Cart> Carts => Set<Cart>();

    public DbSet<CartItem> CartItems => Set<CartItem>();

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Inventory> Inventories => Set<Inventory>();


protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

  modelBuilder.Entity<Product>()
        .Property(p => p.Price)
        .HasPrecision(10, 2);

   modelBuilder.Entity<ProcessedMessage>()
    .HasIndex(x => new
    {
        x.EventId,
        x.ConsumerName
    })
    .IsUnique();

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
    
    modelBuilder.Entity<Payment>()
    .HasIndex(p => p.IdempotencyKey)
    .IsUnique();

    modelBuilder.Entity<OrderItem>()
        .HasOne(oi => oi.Product)
        .WithMany()
        .HasForeignKey(oi => oi.ProductId);

        modelBuilder.Entity<Inventory>()
    .HasOne(i => i.Product)
    .WithOne(p => p.Inventory)
    .HasForeignKey<Inventory>(i => i.ProductId);

    modelBuilder.Entity<Inventory>()
    .HasIndex(i => i.ProductId)
    .IsUnique();
    
    modelBuilder.Entity<Inventory>()
    .Property(i => i.Version)
    .IsConcurrencyToken();

    modelBuilder.Entity<Payment>()
    .HasOne(p => p.Order)
    .WithMany(o => o.Payments)
    .HasForeignKey(p => p.OrderId);

    modelBuilder.Entity<Payment>()
    .HasIndex(p => p.IdempotencyKey)
    .IsUnique();

    modelBuilder.Entity<Inventory>()
    .ToTable(table =>
    {
        table.HasCheckConstraint(
            "CK_Inventory_Quantity_NonNegative",
            "\"Quantity\" >= 0");

        table.HasCheckConstraint(
            "CK_Inventory_ReservedQuantity_NonNegative",
            "\"ReservedQuantity\" >= 0");

        table.HasCheckConstraint(
            "CK_Inventory_Reserved_NotGreaterThan_Quantity",
            "\"ReservedQuantity\" <= \"Quantity\"");
    });
}
}