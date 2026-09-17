namespace FlashCart.Application.Payments;

public class MockPaymentProvider : IPaymentProvider
{
    public async Task<PaymentResult> ProcessPaymentAsync(
        decimal amount,
        string paymentReference,
        CancellationToken cancellationToken)
    {
        // Simulate payment processing
        await Task.Delay(
            TimeSpan.FromSeconds(2),
            cancellationToken);

        // Payment succeeded at provider
        var providerPaymentId = $"mock_{Guid.NewGuid()}";

        Console.WriteLine(
            $"PROVIDER: Payment succeeded. " +
            $"ProviderPaymentId = {providerPaymentId}");

        // Simulate lost response
        await Task.Delay(
            TimeSpan.FromSeconds(10),
            cancellationToken);

        return new PaymentResult
        {
            Success = true,
            ProviderPaymentId = providerPaymentId
        };
    }

    public async Task<PaymentResult> RefundPaymentAsync(
    decimal amount,
    string providerPaymentId,
    CancellationToken cancellationToken)
{
    await Task.Delay(
        TimeSpan.FromSeconds(1),
        cancellationToken);

    Console.WriteLine(
        $"PROVIDER: Refund succeeded for " +
        $"{providerPaymentId}, Amount = {amount}");

    return new PaymentResult
    {
        Success = true,
        ProviderPaymentId = providerPaymentId
    };
}

    public async Task<PaymentResult> VerifyPaymentAsync( string razorpayOrderId,
        string razorpayPaymentId,
        string razorpaySignature,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }



}