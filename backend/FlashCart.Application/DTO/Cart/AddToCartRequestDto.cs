using System.ComponentModel.DataAnnotations;

namespace FlashCart.Application.DTO.Cart;

public class AddToCartRequestDto
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ProductId { get; set; }

    [Required]
    [Range(1, 100)]
    public int Quantity { get; set; }
}