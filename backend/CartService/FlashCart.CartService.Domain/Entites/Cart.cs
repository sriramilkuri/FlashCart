namespace FlashCart.CartService.Domain.Entities;

public class Cart
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public ICollection<CartItem> CartItems { get; set; }
        = new List<CartItem>();

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}