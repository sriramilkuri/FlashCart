using FlashCart.CartService.Application.Product;

namespace FlashCart.CartService.Application.Common.Interfaces;

public interface IProductClient
{
    Task<ProductResponse?> GetProductAsync(
        int productId,
        CancellationToken cancellationToken = default);
}