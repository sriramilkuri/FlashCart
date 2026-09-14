using FlashCart.Domain.Enums;

namespace FlashCart.Domain.Entities;

public class Order
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal Total { get; set; }

    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    public User User { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; }
        = new List<OrderItem>();

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                $"Order cannot be confirmed from {Status} state.");
        }

        Status = OrderStatus.Confirmed;
    }

    public void Cancel()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                $"Order cannot be cancelled from {Status} state.");
        }

        Status = OrderStatus.Cancelled;
    }

    public void Complete()
    {
        if (Status != OrderStatus.Confirmed)
        {
            throw new InvalidOperationException(
                $"Order cannot be completed from {Status} state.");
        }

        Status = OrderStatus.Completed;
    }
}