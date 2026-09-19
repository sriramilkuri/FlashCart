using FlashCart.AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.AuthService.Application.Common.Interfaces;

public interface IAuthDbContext
{
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}