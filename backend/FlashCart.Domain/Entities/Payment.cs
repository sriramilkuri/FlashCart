using FlashCart.Domain.Enums;

namespace FlashCart.Domain.Entities;

public class Payment
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public decimal Amount { get; set; }

    public PaymentStatus Status { get; private set; }
        = PaymentStatus.Pending;

    public string Provider { get; set; } = string.Empty;

    public string? ProviderPaymentId { get; set; }

    public string? IdempotencyKey { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public Order Order { get; set; } = null!;

    public string? ProviderOrderId { get; set; }

    public void StartProcessing()
    {
        if (Status != PaymentStatus.Pending)
        {
            throw new InvalidOperationException(
                $"Payment cannot start processing from {Status} state.");
        }

        Status = PaymentStatus.Processing;
    }

    public void MarkSucceeded()
    {
        if (Status != PaymentStatus.Processing)
        {
            throw new InvalidOperationException(
                $"Payment cannot succeed from {Status} state.");
        }

        Status = PaymentStatus.Succeeded;

        CompletedAt = DateTime.UtcNow;
    }

    public void MarkFailed()
    {
        if (Status != PaymentStatus.Processing)
        {
            throw new InvalidOperationException(
                $"Payment cannot fail from {Status} state.");
        }

        Status = PaymentStatus.Failed;
    }

    public void MarkRefunded()
    {
        if (Status != PaymentStatus.Succeeded)
        {
            throw new InvalidOperationException(
                $"Payment cannot be refunded from {Status} state.");
        }

        Status = PaymentStatus.Refunded;
    }
}