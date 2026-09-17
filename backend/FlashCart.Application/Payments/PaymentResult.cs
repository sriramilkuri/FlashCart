namespace FlashCart.Application.Payments;

public class PaymentResult
{
    public bool Success { get; set; }

    public string? ProviderPaymentId { get; set; }

    public string? ErrorMessage { get; set; }
}