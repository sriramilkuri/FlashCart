namespace FlashCart.CartService.Application.Cart;

public class CartResponse
{
    public int CartId { get; set; }

    public int UserId { get; set; }

    public DateTime UpdatedAt { get; set; }

    public List<CartItemResponse> Items { get; set; }
        = new();
}

public class CartItemResponse
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }
}