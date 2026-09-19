using FlashCart.AuthService.Domain.Entities;

namespace FlashCart.AuthService.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}