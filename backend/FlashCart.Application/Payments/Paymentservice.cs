using FlashCart.Application.Common.Interfaces;
using FlashCart.Domain.Entities;
using FlashCart.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.Application.Payments;

public class PaymentService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPaymentProvider _paymentProvider;

    public PaymentService(
        IApplicationDbContext dbContext,
        IPaymentProvider paymentProvider)
    {
        _dbContext = dbContext;
        _paymentProvider = paymentProvider;
    }

    public async Task<PaymentResponse> ProcessPaymentAsync(
        int orderId,
        int userId,
         string idempotencyKey)
    {
        // 1. Find the order belonging to the logged-in user
        var order = await _dbContext.Orders
            .SingleOrDefaultAsync(o =>
                o.Id == orderId &&
                o.UserId == userId);

        if (order == null)
        {
            throw new KeyNotFoundException(
                "Order not found.");
        }

        // 2. Payment can only be started for a Pending order
        if (order.Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Payment can only be initiated for a pending order.");
        }
        
        var existingPayment =
    await _dbContext.Payments
        .SingleOrDefaultAsync(
            p => p.IdempotencyKey == idempotencyKey);
            if (existingPayment != null)
{
    return new PaymentResponse
    {
        PaymentId = existingPayment.Id,
        OrderId = existingPayment.OrderId,
        Amount = existingPayment.Amount,
        Status = existingPayment.Status.ToString(),
        Provider = existingPayment.Provider,
        Message = "Payment request already processed."
    };
}
        // 3. Create a payment record
        var payment = new Payment
        {
            OrderId = order.Id,
            Amount = order.Total,
            Provider = "Mock",
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Payments.Add(payment);

        // Save so that Payment gets its database Id
        await _dbContext.SaveChangesAsync();

        // 4. Move Payment:
        // Pending → Processing
        payment.StartProcessing();

        await _dbContext.SaveChangesAsync();

        // 5. Set a 5-second timeout
        using var timeoutCts =
            new CancellationTokenSource(
                TimeSpan.FromSeconds(5));

        PaymentResult paymentResult;

        try
        {
            // 6. Call the payment provider
            paymentResult =
                await _paymentProvider.ProcessPaymentAsync(
                    payment.Amount,
                    payment.Id.ToString(),
                    timeoutCts.Token);
        }
        catch (OperationCanceledException)
        {
            // IMPORTANT:
            // Timeout does NOT necessarily mean payment failed.
            //
            // The provider may have processed the payment,
            // but FlashCart did not receive the response.
            //
            // Therefore, leave the payment as Processing.

            return new PaymentResponse
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Status = payment.Status.ToString(),
                Provider = payment.Provider,
                Message =
                    "Payment is still processing. " +
                    "The provider response timed out."
            };
        }

        // 7. Provider explicitly said payment failed
        if (!paymentResult.Success)
        {
            payment.MarkFailed();

            await _dbContext.SaveChangesAsync();

            throw new InvalidOperationException(
                paymentResult.ErrorMessage
                ?? "Payment failed.");
        }

        // 8. Provider successfully processed payment
        payment.ProviderPaymentId =
            paymentResult.ProviderPaymentId;

        // Processing → Succeeded
        payment.MarkSucceeded();

        await _dbContext.SaveChangesAsync();

        // 9. Return successful response
        return new PaymentResponse
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = payment.Status.ToString(),
            Provider = payment.Provider,
            Message = "Payment completed successfully."
        };
    }
}


