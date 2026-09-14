namespace FlashCart.Application.DTO.Orders;

public class CreateOrderRequestDto
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }
}