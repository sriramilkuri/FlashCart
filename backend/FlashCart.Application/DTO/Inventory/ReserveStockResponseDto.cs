namespace FlashCart.Application.DTO.Inventory;

public class ReserveStockResponseDto
{
    public string Message { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public int QuantityReserved { get; set; }

    public int RemainingQuantity { get; set; }
}