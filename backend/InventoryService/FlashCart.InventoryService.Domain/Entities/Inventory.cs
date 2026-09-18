namespace FlashCart.InventoryService.Domain.Entities;

public class Inventory
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int ReservedQuantity { get; set; }

    public DateTime UpdatedAt { get; set; }

    public int Version { get; set; }

    public int AvailableQuantity =>
        Quantity - ReservedQuantity;

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (AvailableQuantity < quantity)
            throw new InvalidOperationException(
                "Insufficient inventory.");

        ReservedQuantity += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Release(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException(
                "Quantity must be greater than zero.");

        if (ReservedQuantity < quantity)
            throw new InvalidOperationException(
                "Cannot release more than reserved quantity.");

        ReservedQuantity -= quantity;
        UpdatedAt = DateTime.UtcNow;
    }
}