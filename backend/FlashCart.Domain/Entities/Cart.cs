namespace FlashCart.Domain.Entities;

public class Cart
{
    public int Id{get;set;}
    public int UserId{get;set;}
    public User User{get;set;} = new User();

    public ICollection<CartItem> CartItems = new List<CartItem>();
}