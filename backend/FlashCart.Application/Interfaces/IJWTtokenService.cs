using FlashCart.Domain.Entities;

namespace FlashCart.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}