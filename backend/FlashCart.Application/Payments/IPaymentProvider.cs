namespace FlashCart.Application.Payments;

public interface IPaymentProvider
{
    Task<PaymentResult> ProcessPaymentAsync(
        decimal amount,
        string paymentReference
        ,CancellationToken cancellationToken);

        Task<PaymentResult> RefundPaymentAsync(
        decimal amount,
        string providerPaymentId,
        CancellationToken cancellationToken);

    Task<PaymentResult> VerifyPaymentAsync(
        string razorpayOrderId,
        string razorpayPaymentId,
        string razorpaySignature,
        CancellationToken cancellationToken);    
}