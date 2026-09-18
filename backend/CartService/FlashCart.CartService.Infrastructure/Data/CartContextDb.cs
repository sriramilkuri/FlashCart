using FlashCart.CartService.Application.Common.Interfaces;
using FlashCart.CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.CartService.Infrastructure.Data;

public class CartDbContext :
    DbContext,
    ICartDbContext
{
    public CartDbContext(
        DbContextOptions<CartDbContext> options)
        : base(options)
    {
    }

    public DbSet<Cart> Carts => Set<Cart>();

    public DbSet<CartItem> CartItems => Set<CartItem>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Cart>()
            .HasIndex(x => x.UserId)
            .IsUnique();

        modelBuilder.Entity<Cart>()
            .HasMany(x => x.CartItems)
            .WithOne(x => x.Cart)
            .HasForeignKey(x => x.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasIndex(x => new
            {
                x.CartId,
                x.ProductId
            })
            .IsUnique();

        modelBuilder.Entity<CartItem>()
            .ToTable(table =>
                table.HasCheckConstraint(
                    "CK_CartItem_Quantity",
                    "\"Quantity\" > 0"));
    }
}