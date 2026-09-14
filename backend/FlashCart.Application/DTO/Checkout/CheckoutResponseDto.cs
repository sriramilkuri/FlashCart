namespace FlashCart.Application.DTO.Checkout;

public class CheckoutResponseDto
{
    public int OrderId { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = string.Empty;
}