namespace FlashCart.Application.DTO.Orders;

public class OrderDetailsResponseDto
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<OrderDetailsItemDto> Items { get; set; }
        = new();
}

public class OrderDetailsItemDto
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal LineTotal { get; set; }
}