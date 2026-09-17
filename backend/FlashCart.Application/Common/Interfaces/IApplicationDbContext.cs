using FlashCart.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Order> Orders { get; }

    DbSet<Payment> Payments { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}