using FlashCart.AuthService.Application.Common.Interfaces;

namespace FlashCart.AuthService.Infrastructure.Security;

public class BcryptPasswordHasher :
    IPasswordHasher
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool Verify(
        string password,
        string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(
            password,
            passwordHash);
    }
}