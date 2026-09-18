using FlashCart.OrderService.Application.Inventory;

namespace FlashCart.OrderService.Application.Common.Interfaces;

public interface IInventoryClient
{
    Task<ReserveInventoryResponse> ReserveAsync(
        int productId,
        int quantity,
        CancellationToken cancellationToken = default);

    Task<ReleaseInventoryResponse> ReleaseAsync(
        int productId,
        int quantity,
        CancellationToken cancellationToken = default);
}