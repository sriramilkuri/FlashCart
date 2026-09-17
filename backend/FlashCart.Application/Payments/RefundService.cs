using FlashCart.Application.Common.Interfaces;
using FlashCart.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace FlashCart.Application.Payments;

public class RefundService
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPaymentProvider _paymentProvider;

    public RefundService(
        IApplicationDbContext dbContext,
        IPaymentProvider paymentProvider)
    {
        _dbContext = dbContext;
        _paymentProvider = paymentProvider;
    }

    public async Task<PaymentResponse> RefundPaymentAsync(
        int paymentId,
        int userId)
    {
        var payment =
            await _dbContext.Payments
                .Include(p => p.Order)
                .SingleOrDefaultAsync(
                    p =>
                        p.Id == paymentId &&
                        p.Order.UserId == userId);

        if (payment == null)
        {
            throw new KeyNotFoundException(
                "Payment not found.");
        }

        if (payment.Status != PaymentStatus.Succeeded)
        {
            throw new InvalidOperationException(
                "Only successful payments can be refunded.");
        }

        if (string.IsNullOrWhiteSpace(
                payment.ProviderPaymentId))
        {
            throw new InvalidOperationException(
                "Provider payment ID is missing.");
        }

        var refundResult =
            await _paymentProvider.RefundPaymentAsync(
                payment.Amount,
                payment.ProviderPaymentId,
                CancellationToken.None);

        if (!refundResult.Success)
        {
            throw new InvalidOperationException(
                refundResult.ErrorMessage
                ?? "Refund failed.");
        }

        payment.MarkRefunded();

        await _dbContext.SaveChangesAsync();

        return new PaymentResponse
        {
            PaymentId = payment.Id,
            OrderId = payment.OrderId,
            Amount = payment.Amount,
            Status = payment.Status.ToString(),
            Provider = payment.Provider,
            Message = "Payment refunded successfully."
        };
    }
}