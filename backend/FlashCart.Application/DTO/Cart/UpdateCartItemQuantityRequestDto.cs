using System.ComponentModel.DataAnnotations;

namespace FlashCart.Application.DTO.Cart;

public class UpdateCartItemQuantityRequestDto
{
    [Range(1, 100)]
    public int Quantity { get; set; }
}