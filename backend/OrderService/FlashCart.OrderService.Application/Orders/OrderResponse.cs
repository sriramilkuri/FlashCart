namespace FlashCart.OrderService.Application.Orders;

public class OrderResponse
{
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<OrderItemResponse> Items { get; set; }
        = new();
}


public class OrderItemResponse
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}