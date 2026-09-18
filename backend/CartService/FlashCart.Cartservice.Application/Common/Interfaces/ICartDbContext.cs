using FlashCart.CartService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CartEntity = FlashCart.CartService.Domain.Entities.Cart;
namespace FlashCart.CartService.Application.Common.Interfaces;

public interface ICartDbContext
{
    DbSet<CartEntity> Carts { get; }

    DbSet<CartItem> CartItems { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}