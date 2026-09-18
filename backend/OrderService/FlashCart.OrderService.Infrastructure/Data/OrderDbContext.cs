using FlashCart.OrderService.Application.Common.Interfaces;
using FlashCart.OrderService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.OrderService.Infrastructure.Data;

public class OrderDbContext :
    DbContext,
    IOrderDbContext
{
    public OrderDbContext(
        DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders
        => Set<Order>();

    public DbSet<OrderItem> OrderItems
        => Set<OrderItem>();


    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<Order>()
            .Property(x => x.Total)
            .HasPrecision(18, 2);


        modelBuilder.Entity<OrderItem>()
            .Property(x => x.UnitPrice)
            .HasPrecision(18, 2);


        modelBuilder.Entity<OrderItem>()
            .HasOne(x => x.Order)
            .WithMany(x => x.OrderItems)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}