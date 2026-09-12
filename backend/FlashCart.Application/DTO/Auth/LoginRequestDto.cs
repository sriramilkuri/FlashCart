using System.ComponentModel.DataAnnotations;

namespace FlashCart.Application.DTOs.Auth;
public class LoginRequestDto
{
    [Required]
    [EmailAddress]
    public string Email{get;set;} = string.Empty;

    [Required]
    public string password{get;set;} = string.Empty;
}