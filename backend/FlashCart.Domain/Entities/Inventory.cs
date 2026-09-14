namespace FlashCart.Domain.Entities;

public class Inventory
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int ReservedQuantity { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Product Product { get; set; } = null!;
    public int Version{get;set;}
}


