using FlashCart.AuthService.Application.Common.Interfaces;
using FlashCart.AuthService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.AuthService.Infrastructure.Data;

public class AuthDbContext :
    DbContext,
    IAuthDbContext
{
    public AuthDbContext(
        DbContextOptions<AuthDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(x => x.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .Property(x => x.Name)
            .HasMaxLength(200);

        modelBuilder.Entity<User>()
            .Property(x => x.Email)
            .HasMaxLength(320);

        modelBuilder.Entity<User>()
            .Property(x => x.PasswordHash)
            .HasMaxLength(500);

        modelBuilder.Entity<User>()
            .Property(x => x.Role)
            .HasMaxLength(50);
    }
}