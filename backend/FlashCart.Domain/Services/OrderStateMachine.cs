using FlashCart.Domain.Enums;

namespace FlashCart.Domain.Services;

public class OrderStateMachine
{
    public bool CanTransition(
        OrderStatus currentStatus,
        OrderStatus newStatus)
    {
        return currentStatus switch
        {
            OrderStatus.Pending =>
                newStatus == OrderStatus.Confirmed ||
                newStatus == OrderStatus.Cancelled,

            OrderStatus.Confirmed =>
                newStatus == OrderStatus.Completed,

            OrderStatus.Cancelled => false,

            OrderStatus.Completed => false,

            _ => false
        };
    }
}