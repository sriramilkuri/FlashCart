namespace FlashCart.Application.DTO.Orders;

public class OrderHistoryResponseDto
{
    public int OrderId { get; set; }

    public DateTime OrderDate { get; set; }

    public decimal Total { get; set; }

    public string Status { get; set; } = string.Empty;

    public List<OrderHistoryItemDto> Items { get; set; } = new();
}