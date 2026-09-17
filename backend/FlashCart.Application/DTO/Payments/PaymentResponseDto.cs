namespace FlashCart.Application.Payments;

public class PaymentResponse
{
    public int PaymentId { get; set; }

    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Provider { get; set; } = string.Empty;

    public string? Message { get; set; }
}