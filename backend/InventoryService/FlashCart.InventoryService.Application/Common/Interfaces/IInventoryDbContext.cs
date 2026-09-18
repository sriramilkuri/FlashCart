using InventoryEntity = FlashCart.InventoryService.Domain.Entities.Inventory;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.InventoryService.Application.Common.Interfaces;

public interface IInventoryDbContext
{
    DbSet<InventoryEntity> Inventories { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}