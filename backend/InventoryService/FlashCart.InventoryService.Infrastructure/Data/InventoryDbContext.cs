using FlashCart.InventoryService.Application.Common.Interfaces;
using FlashCart.InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.InventoryService.Infrastructure.Data;

public class InventoryDbContext :
    DbContext,
    IInventoryDbContext
{
    public InventoryDbContext(
        DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
    }

    public DbSet<Inventory> Inventories
        => Set<Inventory>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Inventory>()
            .HasIndex(x => x.ProductId)
            .IsUnique();

        modelBuilder.Entity<Inventory>()
            .Property(x => x.Version)
            .IsConcurrencyToken();

        modelBuilder.Entity<Inventory>()
            .HasCheckConstraint(
                "CK_Inventory_Quantity",
                "\"Quantity\" >= 0");

        modelBuilder.Entity<Inventory>()
            .HasCheckConstraint(
                "CK_Inventory_ReservedQuantity",
                "\"ReservedQuantity\" >= 0");

        modelBuilder.Entity<Inventory>()
            .HasCheckConstraint(
                "CK_Inventory_ReservedLessThanQuantity",
                "\"ReservedQuantity\" <= \"Quantity\"");
    }
}