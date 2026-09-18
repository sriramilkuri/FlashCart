using FlashCart.ProductService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.ProductService.Application.Common.Interfaces;

public interface IProductDbContext
{
    DbSet<Product> Products { get; }

    DbSet<Category> Categories { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}