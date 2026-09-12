namespace FlashCart.Application.DTOs.Auth;

public class RegisterResponseDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
}