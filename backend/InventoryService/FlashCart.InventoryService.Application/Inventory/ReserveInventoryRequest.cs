namespace FlashCart.InventoryService.Application.Inventory;

public class ReserveInventoryRequest
{
    public int ProductId { get; set; }

    public int Quantity { get; set; }
}