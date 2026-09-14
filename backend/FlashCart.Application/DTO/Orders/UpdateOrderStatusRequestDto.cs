using FlashCart.Domain.Enums;

namespace FlashCart.Application.DTO.Orders;

public class UpdateOrderStatusRequestDto
{
    public OrderStatus Status { get; set; }
}