using OrderEntity =
    FlashCart.OrderService.Domain.Entities.Order;

using OrderItemEntity =
    FlashCart.OrderService.Domain.Entities.OrderItem;

using Microsoft.EntityFrameworkCore;

namespace FlashCart.OrderService.Application.Common.Interfaces;

public interface IOrderDbContext
{
    DbSet<OrderEntity> Orders { get; }

    DbSet<OrderItemEntity> OrderItems { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}