namespace FlashCart.NotificationWorker;

public class OrderCreatedEvent
{
      public Guid EventId { get; set; }
    public int OrderId { get; set; }

    public int UserId { get; set; }

    public decimal Total { get; set; }

    public DateTime CreatedAt { get; set; }
}