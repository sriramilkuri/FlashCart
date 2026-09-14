namespace FlashCart.Application.DTO.Cart;

public class CartResponseDto
{
    public int CartId { get; set; }

    public List<CartItemResponseDto> Items { get; set; } = new();

    public decimal Subtotal { get; set; }

    public decimal Tax { get; set; }

    public decimal TaxRate { get; set; }

    public decimal Total { get; set; }
}