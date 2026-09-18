namespace FlashCart.OrderService.Domain.Entities;

public class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; private set; }

    public Order Order { get; set; } = null!;

    public void SetUnitPrice(decimal price)
    {
        if (price < 0)
        {
            throw new ArgumentException(
                "Price cannot be negative.");
        }

        UnitPrice = price;
    }

    public decimal GetLineTotal()
    {
        return UnitPrice * Quantity;
    }
}