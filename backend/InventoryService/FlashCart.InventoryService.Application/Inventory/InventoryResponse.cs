namespace FlashCart.InventoryService.Application.Inventory;

public class InventoryResponse
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int ReservedQuantity { get; set; }

    public int AvailableQuantity { get; set; }
}